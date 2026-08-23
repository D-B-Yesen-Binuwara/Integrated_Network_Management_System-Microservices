using alarm_service.Correlation.Models;
using alarm_service.Repositories.Interfaces;
using alarm_service.Services.Implement;

namespace alarm_service.Correlation.Engine;

public class ImpactAnalysisEngine
{
    private readonly ITopologyClient _topologyClient;
    private readonly IAlarmFactsProvider _alarmFactsProvider;
    private readonly IRootCauseRepository _rootCauseRepository;
    private readonly ILogger<ImpactAnalysisEngine> _logger;

    public ImpactAnalysisEngine(
        ITopologyClient topologyClient,
        IAlarmFactsProvider alarmFactsProvider,
        IRootCauseRepository rootCauseRepository,
        ILogger<ImpactAnalysisEngine> logger)
    {
        _topologyClient = topologyClient;
        _alarmFactsProvider = alarmFactsProvider;
        _rootCauseRepository = rootCauseRepository;
        _logger = logger;
    }

    public async Task<CorrelationResult> AnalyzeAsync(
        CorrelationContext context,
        CorrelationRule rule,
        CancellationToken cancellationToken = default)
    {
        var result = new CorrelationResult();
        var windowMinutes = Math.Max(rule.WindowMinutes, 1);
        var from = context.RaisedTime.ToUniversalTime().AddMinutes(-windowMinutes);
        var to = context.RaisedTime.ToUniversalTime().AddMinutes(windowMinutes);

        try
        {
            cancellationToken.ThrowIfCancellationRequested();
            var descendants = await _topologyClient.GetDescendantsAsync(context.DeviceId, cancellationToken);
            if (descendants.Count == 0) return result;

            var facts = await _alarmFactsProvider.GetActiveAlarmsAsync(from, to, cancellationToken);
            var activeRootCauses = await _rootCauseRepository.GetAllAsync(cancellationToken);
            var failedParentIds = activeRootCauses.Select(root => root.DeviceId).ToHashSet();
            failedParentIds.Add(context.DeviceId);

            var targetDevices = descendants
                .Where(device => IsTargetDevice(device.DeviceType, rule.TargetDeviceType))
                .GroupBy(device => device.DeviceId)
                .Select(group => group.First())
                .ToList();

            foreach (var device in targetDevices)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (await HasHealthyAlternateParentAsync(device.DeviceId, failedParentIds, facts, cancellationToken))
                {
                    _logger.LogDebug(
                        "Skipping impacted device {DeviceId}; at least one parent remains healthy",
                        device.DeviceId);
                    continue;
                }

                result.ImpactedDevices.Add(device.DeviceId);
            }

            if (rule.SuppressTargetAlarm)
            {
                var impacted = result.ImpactedDevices.ToHashSet();
                result.SuppressedAlarmReferences = facts
                    .Where(fact => impacted.Contains(fact.DeviceId))
                    .Where(fact => IsTargetDevice(fact.DeviceType, rule.TargetDeviceType))
                    .Where(fact => string.Equals(fact.AlarmType, rule.TargetAlarmType, StringComparison.OrdinalIgnoreCase))
                    .Select(fact => new CorrelationAlarmReference
                    {
                        AlarmId = fact.AlarmId,
                        DeviceId = fact.DeviceId,
                        DeviceType = fact.DeviceType,
                        AlarmType = fact.AlarmType,
                        RaisedTime = fact.RaisedTime
                    })
                    .ToList();
                result.SuppressedAlarms = result.SuppressedAlarmReferences.Select(alarm => alarm.AlarmId).ToList();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Impact analysis failed for source device {DeviceId}", context.DeviceId);
            result.Error = "Topology or alarm evidence could not be evaluated.";
        }

        return result;
    }

    private async Task<bool> HasHealthyAlternateParentAsync(
        int deviceId,
        ISet<int> failedParentIds,
        IReadOnlyList<AlarmFact> facts,
        CancellationToken cancellationToken)
    {
        var parents = await _topologyClient.GetParentsAsync(deviceId, cancellationToken);
        if (parents.Count == 0) return false;

        foreach (var parent in parents)
        {
            if (failedParentIds.Contains(parent.DeviceId) || IsFailureStatus(parent.Status)) continue;

            var hasActiveAlarm = facts.Any(fact => fact.DeviceId == parent.DeviceId && fact.IsActive);
            if (!hasActiveAlarm) return true;
        }

        return false;
    }

    private static bool IsTargetDevice(string? actualType, string expectedType) =>
        string.IsNullOrWhiteSpace(expectedType) ||
        string.Equals(actualType, expectedType, StringComparison.OrdinalIgnoreCase);

    private static bool IsFailureStatus(string? status) =>
        string.Equals(status, "DOWN", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(status, "UNREACHABLE", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(status, "IMPACTED", StringComparison.OrdinalIgnoreCase);
}
