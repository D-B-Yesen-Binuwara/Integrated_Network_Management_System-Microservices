using System.Net.Http.Json;
using alarm_service.Interfaces;

namespace alarm_service.Clients;

public class TopologyClient : ITopologyClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TopologyClient> _logger;

    public TopologyClient(HttpClient httpClient, ILogger<TopologyClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IEnumerable<TopologyDeviceDto>> GetParentDevicesAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<TopologyDeviceDto>>($"/api/devices/{deviceId}/parents");
            return response ?? Enumerable.Empty<TopologyDeviceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching parent devices for device {DeviceId}", deviceId);
            return Enumerable.Empty<TopologyDeviceDto>();
        }
    }

    public async Task<IEnumerable<TopologyDeviceDto>> GetChildDevicesAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<TopologyDeviceDto>>($"/api/devices/{deviceId}/children");
            return response ?? Enumerable.Empty<TopologyDeviceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching child devices for device {DeviceId}", deviceId);
            return Enumerable.Empty<TopologyDeviceDto>();
        }
    }

    public async Task<IEnumerable<TopologyLinkDto>> GetAllLinksAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<IEnumerable<TopologyLinkDto>>("/api/device-links");
            return response ?? Enumerable.Empty<TopologyLinkDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching all links");
            return Enumerable.Empty<TopologyLinkDto>();
        }
    }

    public async Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<TopologyDeviceDto>($"/api/devices/{deviceId}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching device {DeviceId}", deviceId);
            return null;
        }
    }
}
