namespace alarm_service.Correlation.Models;

public class AlarmFact
{
    public int AlarmId { get; init; }
    public int DeviceId { get; init; }
    public string DeviceType { get; init; } = string.Empty;
    public string AlarmType { get; init; } = string.Empty;
    public DateTime RaisedTime { get; init; }
    public bool IsActive { get; init; }
}
