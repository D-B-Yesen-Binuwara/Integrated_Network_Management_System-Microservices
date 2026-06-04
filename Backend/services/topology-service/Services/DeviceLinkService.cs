using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.DTOs;
using topology_service.Entities;
using topology_service.Enums;
using topology_service.Repositories;

namespace topology_service.Services;

public class DeviceLinkService : IDeviceLinkService
{
    private readonly IDeviceLinkRepository _repository;
    private readonly TopologyDbContext _context;

    public DeviceLinkService(IDeviceLinkRepository repository, TopologyDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<DeviceLinkDto> CreateLinkAsync(CreateDeviceLinkDto dto)
    {
        if (dto.ParentDeviceId == dto.ChildDeviceId)
            throw new ArgumentException("Parent and child cannot be same");

        var parent = await _context.Devices.FindAsync(dto.ParentDeviceId);
        var child = await _context.Devices.FindAsync(dto.ChildDeviceId);

        if (parent == null || child == null)
            throw new ArgumentException("Device not found");

        if (!IsValidTopology(parent.DeviceType, child.DeviceType))
            throw new InvalidOperationException("Invalid topology: Parent-child relationship not allowed");

        if (await WouldCreateCycleAsync(dto.ParentDeviceId, dto.ChildDeviceId))
            throw new InvalidOperationException("Cycle detected: this link would create a circular dependency");

        var exists = await _context.DeviceLinks.AnyAsync(dl =>
            dl.ParentDeviceId == dto.ParentDeviceId && dl.ChildDeviceId == dto.ChildDeviceId);
        if (exists)
            throw new InvalidOperationException("Link already exists between these devices");

        var link = new DeviceLink
        {
            ParentDeviceId = dto.ParentDeviceId,
            ChildDeviceId = dto.ChildDeviceId,
            LinkStatus = "UP"
        };

        var created = await _repository.AddAsync(link);

        return MapToDto(created);
    }

    public async Task<List<DeviceLinkDto>> GetAllLinksAsync()
    {
        var links = await _repository.GetAllAsync();
        return links.Select(MapToDto).ToList();
    }

    public async Task<bool> DeleteLinkAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    private async Task<bool> WouldCreateCycleAsync(int parentId, int childId)
    {
        // Recursive CTE in PostgreSQL to find if childId is already an ancestor of parentId.
        return await _context.Database
            .SqlQuery<bool>($"""
                WITH RECURSIVE Ancestors AS (
                    SELECT parent_device_id AS "AncestorId"
                    FROM device_links
                    WHERE child_device_id = {parentId}
                    UNION ALL
                    SELECT dl.parent_device_id
                    FROM device_links dl
                    INNER JOIN Ancestors a ON dl.child_device_id = a."AncestorId"
                )
                SELECT EXISTS (
                    SELECT 1 FROM Ancestors WHERE "AncestorId" = {childId}
                )
                """)
            .FirstAsync();
    }

    private bool IsValidTopology(DeviceType parentType, DeviceType childType)
    {
        return parentType switch
        {
            DeviceType.SLBN => childType == DeviceType.SLBN || childType == DeviceType.CEAN,
            DeviceType.CEAN => childType == DeviceType.MSAN || childType == DeviceType.Customer,
            DeviceType.MSAN => childType == DeviceType.Customer,
            _ => false
        };
    }

    private static DeviceLinkDto MapToDto(DeviceLink link)
    {
        return new DeviceLinkDto
        {
            LinkId = link.LinkId,
            ParentDeviceId = link.ParentDeviceId,
            ChildDeviceId = link.ChildDeviceId,
            LinkStatus = link.LinkStatus
        };
    }
}
