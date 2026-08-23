namespace alarm_service.DTOs.Responses;

public class CorrelatedFaultResponse
{
    public int CorrelatedFaultId { get; set; }
    public int RootCauseId { get; set; }
    public string CorrelationRuleName { get; set; } = string.Empty;
    public int SourceDeviceId { get; set; }
    public string SourceDeviceType { get; set; } = string.Empty;
    public int SourceAlarmId { get; set; }
    public string SourceAlarmType { get; set; } = string.Empty;
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal ConfidenceScore { get; set; }
    public List<SuppressedAlarmResponse> SuppressedAlarms { get; set; } = [];
}

public class SuppressedAlarmResponse
{
    public int AlarmId { get; set; }
    public int DeviceId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
    public string AlarmType { get; set; } = string.Empty;
    public DateTime RaisedTime { get; set; }
}
