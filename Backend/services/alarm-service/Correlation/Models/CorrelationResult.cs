namespace alarm_service.Correlation.Models;

public class CorrelationResult
{
    public int? RootCauseDeviceId { get; set; }
    public int? RootCauseAlarmId { get; set; }
    public List<int> ImpactedDevices { get; set; } = [];
    public List<int> SuppressedAlarms { get; set; } = [];
    public DateTime CorrelationTime { get; set; } = DateTime.UtcNow;
}
