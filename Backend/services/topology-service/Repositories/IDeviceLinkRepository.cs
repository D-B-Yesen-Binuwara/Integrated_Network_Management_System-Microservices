using topology_service.Entities;

namespace topology_service.Repositories;

public interface IDeviceLinkRepository
{
    Task<DeviceLink> AddAsync(DeviceLink link);
    Task<List<DeviceLink>> GetAllAsync();
    Task<DeviceLink?> GetByIdAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<List<DeviceLink>> GetChildLinksAsync(int parentDeviceId);
    Task<List<DeviceLink>> GetParentLinksAsync(int childDeviceId);
}
