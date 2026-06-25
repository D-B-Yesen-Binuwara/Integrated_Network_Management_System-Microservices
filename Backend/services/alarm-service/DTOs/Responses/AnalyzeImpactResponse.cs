namespace alarm_service.DTOs.Responses;

public class AnalyzeImpactResponse
{
    public int RootCauseId { get; set; }
    public int RootDeviceId { get; set; }
    public List<int> ImpactedDeviceIds { get; set; } = new List<int>();
}
