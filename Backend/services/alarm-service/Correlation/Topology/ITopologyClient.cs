using alarm_service.Correlation.Models;

namespace alarm_service.Correlation.Topology;

public interface ITopologyClient
{
    Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetChildrenAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetParentsAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetDescendantsAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetAncestorsAsync(int deviceId);
}
