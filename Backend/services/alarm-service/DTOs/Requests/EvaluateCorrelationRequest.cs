namespace alarm_service.DTOs.Requests;

public class EvaluateCorrelationRequest
{
    public int AlarmId { get; set; }
    public int DeviceId { get; set; }
    public string AlarmType { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public DateTime? RaisedTime { get; set; }
}
