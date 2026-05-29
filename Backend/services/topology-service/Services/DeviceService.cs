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

    public IEnumerable<DeviceDto> GetAll()
    {
        return _deviceRepository.GetAll().Select(MapToDto);
    }

    public DeviceDto? GetById(int id)
    {
        var device = _deviceRepository.GetById(id);
        return device == null ? null : MapToDto(device);
    }

    public DeviceDto Create(CreateDeviceDto dto)
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

        var created = _deviceRepository.Create(device);
        return MapToDto(created);
    }

    public DeviceDto? Update(int id, UpdateDeviceDto dto)
    {
        var existing = _deviceRepository.GetById(id);
        if (existing == null)
        {
            return null;
        }

        existing.DeviceName = dto.DeviceName;
        existing.DeviceType = dto.DeviceType;
        existing.IP = dto.IP ?? string.Empty;
        existing.Status = dto.Status;
        existing.PriorityLevel = dto.PriorityLevel;
        existing.Latitude = dto.Latitude;
        existing.Longitude = dto.Longitude;

        var updated = _deviceRepository.Update(id, existing);
        return updated == null ? null : MapToDto(updated);
    }

    public bool Delete(int id)
    {
        return _deviceRepository.Delete(id);
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
