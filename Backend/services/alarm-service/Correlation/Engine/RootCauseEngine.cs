using alarm_service.Correlation.Models;

namespace alarm_service.Correlation.Engine;

public class RootCauseEngine(RuleLoader ruleLoader)
{
    public CorrelationResult Evaluate(CorrelationContext context)
    {
        var result = new CorrelationResult();

        var matchedRule = ruleLoader.GetAllRules()
            .FirstOrDefault(r =>
                r.SourceAlarmType == context.AlarmType &&
                r.SourceDeviceType == context.DeviceType);

        if (matchedRule is null || !matchedRule.MarkSourceAsRootCause)
            return result;

        result.RootCauseDeviceId = context.DeviceId;
        result.RootCauseAlarmId = context.AlarmId;

        return result;
    }
}
