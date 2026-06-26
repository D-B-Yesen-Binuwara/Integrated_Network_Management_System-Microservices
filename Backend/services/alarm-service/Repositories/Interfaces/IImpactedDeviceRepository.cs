using alarm_service.Entities;

namespace alarm_service.Repositories.Interfaces;

public interface IImpactedDeviceRepository
{
    Task<IEnumerable<ImpactedDevice>> GetByRootCauseIdAsync(int rootCauseId);
    Task<IEnumerable<ImpactedDevice>> GetByDeviceIdAsync(int deviceId);
    Task<ImpactedDevice> CreateAsync(ImpactedDevice impactedDevice);
    Task CreateRangeAsync(IEnumerable<ImpactedDevice> impactedDevices);
    Task DeleteAsync(int id);
    Task DeleteByRootCauseAsync(int rootCauseId);
}
