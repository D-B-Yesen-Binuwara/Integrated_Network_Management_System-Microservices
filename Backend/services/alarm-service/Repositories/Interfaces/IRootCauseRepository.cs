using alarm_service.Entities;

namespace alarm_service.Repositories.Interfaces;

public interface IRootCauseRepository
{
    Task<RootCause?> GetByIdAsync(int id);
    Task<RootCause?> GetByDeviceIdAsync(int deviceId);
    Task<IEnumerable<RootCause>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RootCause> CreateAsync(RootCause rootCause);
    Task DeleteAsync(int id);
}
