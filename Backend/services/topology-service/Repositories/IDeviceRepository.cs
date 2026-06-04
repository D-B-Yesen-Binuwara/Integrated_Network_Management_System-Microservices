using topology_service.Entities;

namespace topology_service.Repositories;

public interface IDeviceRepository
{
    Task<List<Device>> GetAllAsync();
    Task<Device?> GetByIdAsync(int id);
    Task AddAsync(Device device);
    Task<Device?> UpdateAsync(int id, Device device);
    Task<bool> DeleteAsync(int id);
}
