namespace alarm_service.Correlation.Models;

public class CorrelationResult
{
    public int SourceAlarmId { get; set; }
    public int SourceDeviceId { get; set; }
    public string SourceAlarmType { get; set; } = string.Empty;
    public string SourceDeviceType { get; set; } = string.Empty;
    public string? MatchedRuleName { get; set; }
    public int? MatchedRulePriority { get; set; }
    public string? TargetAlarmType { get; set; }
    public string? TargetDeviceType { get; set; }
    public int? RootCauseDeviceId { get; set; }
    public int? RootCauseAlarmId { get; set; }
    public List<int> ImpactedDevices { get; set; } = [];
    public List<int> SuppressedAlarms { get; set; } = [];
    public List<CorrelationAlarmReference> SuppressedAlarmReferences { get; set; } = [];
    public int? CorrelatedFaultId { get; set; }
    public string? Error { get; set; }
    public DateTime CorrelationTime { get; set; } = DateTime.UtcNow;
}

public class CorrelationAlarmReference
{
    public int AlarmId { get; set; }
    public int DeviceId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public DateTime RaisedTime { get; set; }
}
