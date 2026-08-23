using alarm_service.DTOs;

namespace alarm_service.Services.Implement;

public interface ITopologyClient
{
    Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId, CancellationToken cancellationToken = default);
    Task<List<TopologyDeviceDto>> GetChildrenAsync(int deviceId, CancellationToken cancellationToken = default);
    Task<List<TopologyDeviceDto>> GetParentsAsync(int deviceId, CancellationToken cancellationToken = default);
    Task<List<TopologyDeviceDto>> GetDescendantsAsync(int deviceId, CancellationToken cancellationToken = default);
    Task<List<TopologyDeviceDto>> GetAncestorsAsync(int deviceId, CancellationToken cancellationToken = default);
}
