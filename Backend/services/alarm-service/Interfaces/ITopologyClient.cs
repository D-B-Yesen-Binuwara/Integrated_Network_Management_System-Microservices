namespace alarm_service.Clients;

public class TopologyLinkDto
{
    public int ParentDeviceId { get; set; }
    public int ChildDeviceId { get; set; }
}

public class TopologyDeviceDto
{
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public interface ITopologyClient
{
    Task<IEnumerable<TopologyDeviceDto>> GetParentDevicesAsync(int deviceId);
    Task<IEnumerable<TopologyDeviceDto>> GetChildDevicesAsync(int deviceId);
    Task<IEnumerable<TopologyLinkDto>> GetAllLinksAsync();
    Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId);
}
