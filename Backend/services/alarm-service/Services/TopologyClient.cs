using System.Net.Http.Json;
using alarm_service.DTOs;
using alarm_service.Services.Implement;

namespace alarm_service.Services;

public class TopologyClient : ITopologyClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<TopologyClient> _logger;

    public TopologyClient(HttpClient httpClient, ILogger<TopologyClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<TopologyDeviceDto?> GetDeviceAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<TopologyDeviceDto>($"/api/device/{deviceId}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching device {DeviceId}", deviceId);
            return null;
        }
    }

    public async Task<List<TopologyDeviceDto>> GetChildrenAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TopologyDeviceDto>>($"/api/device/{deviceId}/children");
            return response ?? new List<TopologyDeviceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching child devices for device {DeviceId}", deviceId);
            return new List<TopologyDeviceDto>();
        }
    }

    public async Task<List<TopologyDeviceDto>> GetParentsAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TopologyDeviceDto>>($"/api/device/{deviceId}/parents");
            return response ?? new List<TopologyDeviceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching parent devices for device {DeviceId}", deviceId);
            return new List<TopologyDeviceDto>();
        }
    }

    public async Task<List<TopologyDeviceDto>> GetDescendantsAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TopologyDeviceDto>>($"/api/device/{deviceId}/descendants");
            return response ?? new List<TopologyDeviceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching descendant devices for device {DeviceId}", deviceId);
            return new List<TopologyDeviceDto>();
        }
    }

    public async Task<List<TopologyDeviceDto>> GetAncestorsAsync(int deviceId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<TopologyDeviceDto>>($"/api/device/{deviceId}/ancestors");
            return response ?? new List<TopologyDeviceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching ancestor devices for device {DeviceId}", deviceId);
            return new List<TopologyDeviceDto>();
        }
    }
}
