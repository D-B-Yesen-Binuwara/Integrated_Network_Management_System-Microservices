using alarm_service.Entities;

namespace alarm_service.Interfaces;

public interface IRootCauseRepository
{
    Task<RootCause?> GetByIdAsync(int id);
    Task<RootCause?> GetByDeviceIdAsync(int deviceId);
    Task<IEnumerable<RootCause>> GetAllAsync();
    Task<RootCause> CreateAsync(RootCause rootCause);
    Task DeleteAsync(int id);
}
