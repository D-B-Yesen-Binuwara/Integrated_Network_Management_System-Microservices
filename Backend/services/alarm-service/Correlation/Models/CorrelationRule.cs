namespace alarm_service.Correlation.Models;

public class CorrelationRule
{
    public string RuleName { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public int Priority { get; set; }
    public string SourceAlarmType { get; set; } = string.Empty;
    public string SourceDeviceType { get; set; } = string.Empty;
    public string TargetAlarmType { get; set; } = string.Empty;
    public string TargetDeviceType { get; set; } = string.Empty;
    public bool MarkSourceAsRootCause { get; set; }
    public bool SuppressTargetAlarm { get; set; }
}
