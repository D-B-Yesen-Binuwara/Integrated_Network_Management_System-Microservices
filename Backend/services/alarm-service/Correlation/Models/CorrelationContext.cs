namespace alarm_service.Correlation.Models;

public class CorrelationContext
{
    public int AlarmId { get; set; }
    public string AlarmType { get; set; } = string.Empty;
    public int DeviceId { get; set; }
    public string DeviceType { get; set; } = string.Empty;
}
