using alarm_service.Correlation.Models;

namespace alarm_service.Correlation.Engine;

public interface ICorrelationEngine
{
    Task<CorrelationResult> EvaluateAsync(
        CorrelationContext context,
        CancellationToken cancellationToken = default);
}
