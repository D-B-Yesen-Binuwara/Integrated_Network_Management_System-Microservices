using alarm_service.DTOs;

namespace alarm_service.Services.Implement;

public interface ITopologyClient
{
    Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetChildrenAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetParentsAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetDescendantsAsync(int deviceId);
    Task<List<TopologyDeviceDto>> GetAncestorsAsync(int deviceId);
}
