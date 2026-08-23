using alarm_service.Correlation.Models;

namespace alarm_service.Correlation.Engine;

public class RootCauseEngine(RuleLoader ruleLoader)
{
    public CorrelationResult Evaluate(CorrelationContext context, CorrelationRule? matchedRule = null)
    {
        var result = new CorrelationResult();

        // Reuse the orchestrator's rule when available; otherwise keep this engine usable on its own.
        matchedRule ??= ruleLoader.FindMatchingRule(context);

        if (matchedRule is null || !matchedRule.MarkSourceAsRootCause)
            return result;

        result.RootCauseDeviceId = context.DeviceId;
        result.RootCauseAlarmId = context.AlarmId;

        return result;
    }
}
