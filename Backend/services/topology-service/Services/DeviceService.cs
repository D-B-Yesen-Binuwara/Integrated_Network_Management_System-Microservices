using topology_service.DTOs;
using topology_service.Entities;
using topology_service.Enums;
using topology_service.Repositories;

namespace topology_service.Services;

public class DeviceService : IDeviceService
{
    private readonly IDeviceRepository _deviceRepository;

    public DeviceService(IDeviceRepository deviceRepository)
    {
        _deviceRepository = deviceRepository;
    }

    public async Task<List<DeviceDto>> GetAllAsync()
    {
        var devices = await _deviceRepository.GetAllAsync();
        return devices.Select(MapToDto).ToList();
    }

    public async Task<DeviceDto?> GetByIdAsync(int id)
    {
        var device = await _deviceRepository.GetByIdAsync(id);
        return device == null ? null : MapToDto(device);
    }

    public async Task<DeviceDto> CreateAsync(CreateDeviceDto dto)
    {
        var device = new Device
        {
            DeviceName = dto.DeviceName,
            DeviceType = dto.DeviceType,
            IP = dto.IP ?? string.Empty,
            Status = DeviceStatus.UP,
            PriorityLevel = dto.PriorityLevel,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _deviceRepository.AddAsync(device);
        return MapToDto(device);
    }

    public async Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto dto)
    {
        var device = new Device
        {
            DeviceName = dto.DeviceName,
            DeviceType = dto.DeviceType,
            IP = dto.IP ?? string.Empty,
            Status = dto.Status,
            PriorityLevel = dto.PriorityLevel,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        var updated = await _deviceRepository.UpdateAsync(id, device);
        return updated == null ? null : MapToDto(updated);
    }

    public Task<bool> DeleteAsync(int id)
    {
        return _deviceRepository.DeleteAsync(id);
    }

    private static DeviceDto MapToDto(Device device)
    {
        return new DeviceDto
        {
            DeviceId = device.DeviceId,
            DeviceName = device.DeviceName,
            DeviceType = device.DeviceType,
            IP = device.IP,
            Status = device.Status,
            PriorityLevel = device.PriorityLevel,
            Latitude = device.Latitude,
            Longitude = device.Longitude
        };
    }
}
