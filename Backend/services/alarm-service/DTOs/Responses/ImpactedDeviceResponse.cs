namespace alarm_service.DTOs.Responses;

public class ImpactedDeviceResponse
{
    public int DeviceId { get; set; }
    public string ImpactType { get; set; } = string.Empty;
}
