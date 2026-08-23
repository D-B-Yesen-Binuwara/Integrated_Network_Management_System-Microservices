using alarm_service.Correlation.Models;

namespace alarm_service.Services.Implement;

public interface ICorrelationResultService
{
    Task PersistAsync(CorrelationResult result, CancellationToken cancellationToken = default);
    Task ClearForDeviceAsync(int deviceId, CancellationToken cancellationToken = default);
}
