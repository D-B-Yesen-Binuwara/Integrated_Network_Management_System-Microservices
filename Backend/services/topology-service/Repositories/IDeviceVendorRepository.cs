using topology_service.Entities;

namespace topology_service.Repositories;

public interface IDeviceVendorRepository
{
    Task<DeviceVendor?> GetByIdAsync(int id);
    Task<List<DeviceVendor>> GetByVendorIdAsync(int vendorId);
    Task<List<DeviceVendor>> GetByDeviceIdAsync(int deviceId);
    Task<int> GetActiveDeviceCountAsync(int vendorId);
    Task<int> GetTotalDeviceCountAsync(int vendorId);
    Task<DateTime?> GetLastAssignmentDateAsync(int vendorId);
    Task<List<DeviceVendor>> GetRecentAssignmentsAsync(int vendorId, int limit = 5);
    Task AddAsync(DeviceVendor deviceVendor);
    Task UpdateAsync(DeviceVendor deviceVendor);
    Task DeleteAsync(DeviceVendor deviceVendor);
    Task<bool> ExistsAsync(int deviceId, int vendorId);
}
