using alarm_service.Correlation.Models;
using alarm_service.Services.Implement;

namespace alarm_service.Correlation.Engine;

public class CorrelationEngine : ICorrelationEngine
{
    private readonly RuleLoader _ruleLoader;
    private readonly RootCauseEngine _rootCauseEngine;
    private readonly ImpactAnalysisEngine _impactAnalysisEngine;
    private readonly ICorrelationResultService _resultService;
    private readonly ILogger<CorrelationEngine> _logger;

    public CorrelationEngine(
        RuleLoader ruleLoader,
        RootCauseEngine rootCauseEngine,
        ImpactAnalysisEngine impactAnalysisEngine,
        ICorrelationResultService resultService,
        ILogger<CorrelationEngine> logger)
    {
        _ruleLoader = ruleLoader;
        _rootCauseEngine = rootCauseEngine;
        _impactAnalysisEngine = impactAnalysisEngine;
        _resultService = resultService;
        _logger = logger;
    }

    public async Task<CorrelationResult> EvaluateAsync(
        CorrelationContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);

        var result = new CorrelationResult
        {
            SourceAlarmId = context.AlarmId,
            SourceDeviceId = context.DeviceId,
            SourceAlarmType = context.AlarmType,
            SourceDeviceType = context.DeviceType
        };

        // Find the first enabled rule because RuleLoader already orders rules by priority.
        var matchedRule = _ruleLoader.FindMatchingRule(context);
        if (matchedRule is null)
        {
            _logger.LogDebug(
                "No correlation rule matched AlarmType {AlarmType} and DeviceType {DeviceType}",
                context.AlarmType,
                context.DeviceType);

            return result;
        }

        result.MatchedRuleName = matchedRule.RuleName;
        result.MatchedRulePriority = matchedRule.Priority;
        result.TargetAlarmType = matchedRule.TargetAlarmType;
        result.TargetDeviceType = matchedRule.TargetDeviceType;

        cancellationToken.ThrowIfCancellationRequested();

        // Evaluate root cause using the same rule so the rule is not selected twice differently.
        var rootCauseResult = _rootCauseEngine.Evaluate(context, matchedRule);
        result.RootCauseDeviceId = rootCauseResult.RootCauseDeviceId;
        result.RootCauseAlarmId = rootCauseResult.RootCauseAlarmId;

        if (!result.RootCauseDeviceId.HasValue)
        {
            return result;
        }

        cancellationToken.ThrowIfCancellationRequested();

        // Topology traversal returns candidate downstream devices; alarm evidence and persistence are later stages.
        var impactResult = await _impactAnalysisEngine.AnalyzeAsync(context, matchedRule, cancellationToken);
        result.ImpactedDevices = impactResult.ImpactedDevices;
        result.SuppressedAlarms = impactResult.SuppressedAlarms;
        result.SuppressedAlarmReferences = impactResult.SuppressedAlarmReferences;
        result.Error = impactResult.Error;

        await _resultService.PersistAsync(result, cancellationToken);

        _logger.LogInformation(
            "Correlation rule {RuleName} matched source device {DeviceId}; found {ImpactedCount} impacted device candidates",
            matchedRule.RuleName,
            context.DeviceId,
            result.ImpactedDevices.Count);

        return result;
    }
}
