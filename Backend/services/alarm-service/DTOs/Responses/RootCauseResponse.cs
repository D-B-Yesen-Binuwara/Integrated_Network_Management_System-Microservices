namespace alarm_service.DTOs.Responses;

public class RootCauseResponse
{
    public int RootCauseId { get; set; }
    public int DeviceId { get; set; }
    public string RootCauseType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
