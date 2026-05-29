using topology_service.DTOs;

namespace topology_service.Services;

public interface IDeviceService
{
    Task<List<DeviceDto>> GetAllAsync();
    Task<DeviceDto?> GetByIdAsync(int id);
    Task<DeviceDto> CreateAsync(CreateDeviceDto dto);
    Task<DeviceDto?> UpdateAsync(int id, UpdateDeviceDto dto);
    Task<bool> DeleteAsync(int id);
}
