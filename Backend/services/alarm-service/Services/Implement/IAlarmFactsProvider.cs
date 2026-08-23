using alarm_service.Correlation.Models;

namespace alarm_service.Services.Implement;

public interface IAlarmFactsProvider
{
    Task<IReadOnlyList<AlarmFact>> GetActiveAlarmsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default);
}
