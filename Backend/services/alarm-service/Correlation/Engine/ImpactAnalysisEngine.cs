using alarm_service.Correlation.Models;
using alarm_service.Services.Implement;

namespace alarm_service.Correlation.Engine;

public class ImpactAnalysisEngine
{
    private readonly ITopologyClient _topologyClient;
    private readonly ILogger<ImpactAnalysisEngine> _logger;

    public ImpactAnalysisEngine(ITopologyClient topologyClient, ILogger<ImpactAnalysisEngine> logger)
    {
        _topologyClient = topologyClient;
        _logger = logger;
    }

    public async Task<CorrelationResult> AnalyzeAsync(CorrelationContext context, CorrelationRule? rule = null)
    {
        var result = new CorrelationResult();

        try
        {
            _logger.LogDebug("ImpactAnalysisEngine: fetching descendants for DeviceId {DeviceId}", context.DeviceId);

            var descendants = await _topologyClient.GetDescendantsAsync(context.DeviceId);
            if (descendants is null || !descendants.Any())
            {
                return result;
            }

            result.ImpactedDevices = descendants.Select(d => d.DeviceId).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ImpactAnalysisEngine failed for DeviceId {DeviceId}", context.DeviceId);
        }

        return result;
    }
}
