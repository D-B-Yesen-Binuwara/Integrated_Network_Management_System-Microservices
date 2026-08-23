using topology_service.DTOs;
using topology_service.Entities;
using topology_service.Enums;
using topology_service.Repositories;

namespace topology_service.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;
    private readonly ILEARepository _leaRepository;

    public DeviceService(IDeviceRepository deviceRepository, ILEARepository leaRepository)
    {
        _deviceRepository = deviceRepository;
        _leaRepository = leaRepository;
    }

    public async Task<List<DeviceDto>> GetAllAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        var leas = await _leaRepository.GetAllAsync();
        return devices.Select(device => MapToDto(device, leas)).ToList();
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        if (device == null) return null;

        var leas = await _leaRepository.GetAllAsync();
        return MapToDto(device, leas);
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto)
    {
        var lea = await ResolveLeaAsync(dto.LEACode);
        var device = new Device
        {
            DeviceName = dto.DeviceName,
            DeviceType = dto.DeviceType,
            IP = dto.IP ?? string.Empty,
            RegionCode = lea.Province?.Region?.RegionCode ?? string.Empty,
            ProvinceCode = lea.Province?.ProvinceCode ?? string.Empty,
            LEACode = lea.LEACode,
            AssignedEngineerId = dto.AssignedEngineerId,
            Status = DeviceStatus.UP,
            PriorityLevel = dto.PriorityLevel,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _deviceRepository.AddAsync(device);
        return MapToDto(device, [lea]);
    }

    public async Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var lea = await ResolveLeaAsync(dto.LEACode);
        var device = new Device
        {
            DeviceName = dto.DeviceName,
            DeviceType = dto.DeviceType,
            IP = dto.IP ?? string.Empty,
            RegionCode = lea.Province?.Region?.RegionCode ?? string.Empty,
            ProvinceCode = lea.Province?.ProvinceCode ?? string.Empty,
            LEACode = lea.LEACode,
            AssignedEngineerId = dto.AssignedEngineerId,
            Status = dto.Status,
            PriorityLevel = dto.PriorityLevel,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        var updated = await _deviceRepository.UpdateAsync(id, device);
        return updated == null ? null : MapToDto(updated, [lea]);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _deviceRepository.DeleteAsync(id);
    }

    private async Task<LEA> ResolveLeaAsync(string leaCode)
    {
        if (string.IsNullOrWhiteSpace(leaCode))
            throw new ArgumentException("An LEA code is required when creating or updating a device.");

        var lea = (await _leaRepository.GetAllAsync())
            .FirstOrDefault(item => string.Equals(item.LEACode, leaCode.Trim(), StringComparison.OrdinalIgnoreCase));

        if (lea?.Province?.Region is null)
            throw new InvalidOperationException($"LEA code '{leaCode}' does not resolve to a province and region.");

        return lea;
    }

    private static DeviceDto MapToDto(Device device, IEnumerable<LEA> leas)
    {
        var lea = leas.FirstOrDefault(item => string.Equals(item.LEACode, device.LEACode, StringComparison.OrdinalIgnoreCase));
        return new DeviceDto
        {
            DeviceId = device.DeviceId,
            DeviceName = device.DeviceName,
            DeviceType = device.DeviceType,
            IP = device.IP,
            RegionCode = device.RegionCode,
            ProvinceCode = device.ProvinceCode,
            LEACode = device.LEACode,
            RegionName = lea?.Province?.Region?.Name ?? string.Empty,
            ProvinceName = lea?.Province?.Name ?? string.Empty,
            LEAName = lea?.Name ?? string.Empty,
            AssignedEngineerId = device.AssignedEngineerId,
            Status = device.Status,
            PriorityLevel = device.PriorityLevel,
            Latitude = device.Latitude,
            Longitude = device.Longitude
        };
    }
}
