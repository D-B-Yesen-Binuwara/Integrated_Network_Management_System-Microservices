using topology_service.DTOs;
using topology_service.Entities;

namespace topology_service.Services;

public interface IDeviceService
{
    IEnumerable<DeviceDto> GetAll();
    DeviceDto? GetById(int id);
    DeviceDto Create(CreateDeviceDto dto);
    DeviceDto? Update(int id, UpdateDeviceDto dto);
    bool Delete(int id);
}
