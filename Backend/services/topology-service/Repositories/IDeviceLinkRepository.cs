using topology_service.Entities;

namespace topology_service.Repositories;

public interface IDeviceLinkRepository
{
    Task<DeviceLink> AddAsync(DeviceLink link);
    Task<List<DeviceLink>> GetAllAsync();
    Task<DeviceLink?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
}
