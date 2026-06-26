using alarm_service.DTOs.Responses;
using alarm_service.Entities;
using alarm_service.Repositories.Interfaces;
using alarm_service.Services.Implement;

namespace alarm_service.Services;

public class ImpactAnalysisService : IImpactAnalysisService
{
    private readonly IRootCauseRepository _rootCauseRepository;
    private readonly IImpactedDeviceRepository _impactedDeviceRepository;
    private readonly ITopologyClient _topologyClient;
    private readonly ILogger<ImpactAnalysisService> _logger;

    public ImpactAnalysisService(
        IRootCauseRepository rootCauseRepository,
        IImpactedDeviceRepository impactedDeviceRepository,
        ITopologyClient topologyClient,
        ILogger<ImpactAnalysisService> logger)
    {
        _rootCauseRepository = rootCauseRepository;
        _impactedDeviceRepository = impactedDeviceRepository;
        _topologyClient = topologyClient;
        _logger = logger;
    }

    public async Task<AnalyzeImpactResponse> AnalyzeFailureAsync(int deviceId, int alarmId)
    {
        _logger.LogInformation("Analyzing failure for DeviceId {DeviceId} and AlarmId {AlarmId}", deviceId, alarmId);

        var parentDevices = await _topologyClient.GetParentsAsync(deviceId);
        bool isRootFailure = true;

        if (parentDevices.Any())
        {
            var activeRootCauses = await _rootCauseRepository.GetAllAsync();
            var parentIds = parentDevices.Select(p => p.DeviceId).ToHashSet();
            
            if (activeRootCauses.Any(rc => parentIds.Contains(rc.DeviceId)))
            {
                isRootFailure = false;
            }
        }

        RootCause rootCause;
        if (isRootFailure)
        {
            rootCause = await _rootCauseRepository.GetByDeviceIdAsync(deviceId);
            if (rootCause == null)
            {
                rootCause = await _rootCauseRepository.CreateAsync(new RootCause
                {
                    DeviceId = deviceId,
                    AlarmId = alarmId,
                    RootCauseType = "NODE_DOWN"
                });
            }
        }
        else
        {
            var activeRootCauses = await _rootCauseRepository.GetAllAsync();
            rootCause = await FindUpstreamRootCause(deviceId, activeRootCauses);
            if (rootCause == null)
            {
                rootCause = await _rootCauseRepository.CreateAsync(new RootCause
                {
                    DeviceId = deviceId,
                    AlarmId = alarmId,
                    RootCauseType = "NODE_DOWN"
                });
            }
        }

        var impactedDeviceIds = await GetDownstreamDeviceIdsAsync(rootCause.DeviceId);
        await RebuildImpactedDevicesAsync(rootCause.RootCauseId, rootCause.DeviceId, impactedDeviceIds);

        return new AnalyzeImpactResponse
        {
            RootCauseId = rootCause.RootCauseId,
            RootDeviceId = rootCause.DeviceId,
            ImpactedDeviceIds = impactedDeviceIds.ToList()
        };
    }

    public async Task<IEnumerable<RootCauseResponse>> GetRootCausesAsync()
    {
        var rootCauses = await _rootCauseRepository.GetAllAsync();
        return rootCauses.Select(rc => new RootCauseResponse
        {
            RootCauseId = rc.RootCauseId,
            DeviceId = rc.DeviceId,
            RootCauseType = rc.RootCauseType,
            CreatedAt = rc.CreatedAt
        });
    }

    public async Task<IEnumerable<ImpactedDeviceResponse>> GetImpactedDevicesAsync(int rootCauseId)
    {
        var impactedDevices = await _impactedDeviceRepository.GetByRootCauseIdAsync(rootCauseId);
        return impactedDevices.Select(id => new ImpactedDeviceResponse
        {
            DeviceId = id.DeviceId,
            ImpactType = id.ImpactType
        });
    }

    public async Task ClearRootCauseAsync(int deviceId)
    {
        _logger.LogInformation("Clearing root cause for DeviceId {DeviceId}", deviceId);
        
        var rootCause = await _rootCauseRepository.GetByDeviceIdAsync(deviceId);
        if (rootCause == null) return;
        
        var impactedDevices = await _impactedDeviceRepository.GetByRootCauseIdAsync(rootCause.RootCauseId);
        var impactedDeviceIds = impactedDevices.Select(i => i.DeviceId).ToList();
        
        await _rootCauseRepository.DeleteAsync(rootCause.RootCauseId);
        
        if (impactedDeviceIds.Any())
        {
            await ReevaluateDownstreamFailuresAsync(impactedDeviceIds);
        }
    }

    public async Task ReevaluateDownstreamFailuresAsync(IEnumerable<int> previouslyImpactedDeviceIds)
    {
        _logger.LogInformation("Reevaluating downstream failures for {Count} devices", previouslyImpactedDeviceIds.Count());
        var activeRootCauses = (await _rootCauseRepository.GetAllAsync()).ToList();
        
        foreach (var deviceId in previouslyImpactedDeviceIds)
        {
            var device = await _topologyClient.GetDeviceAsync(deviceId);
            if (device != null && device.Status == "DOWN")
            {
                var existingRc = activeRootCauses.FirstOrDefault(rc => rc.DeviceId == deviceId);
                if (existingRc != null) continue;
                
                var upstreamRc = await FindUpstreamRootCause(deviceId, activeRootCauses);
                if (upstreamRc == null)
                {
                    _logger.LogInformation("Promoting DeviceId {DeviceId} to root cause", deviceId);
                    await AnalyzeFailureAsync(deviceId, 0);
                    activeRootCauses = (await _rootCauseRepository.GetAllAsync()).ToList();
                }
            }
        }
    }

    public async Task RebuildImpactAsync(int deviceId)
    {
        _logger.LogInformation("Rebuilding impact for DeviceId {DeviceId}", deviceId);
        var rootCause = await _rootCauseRepository.GetByDeviceIdAsync(deviceId);
        if (rootCause == null) return;
        
        var impactedDeviceIds = await GetDownstreamDeviceIdsAsync(deviceId);
        await RebuildImpactedDevicesAsync(rootCause.RootCauseId, deviceId, impactedDeviceIds);
    }

    private async Task<RootCause?> FindUpstreamRootCause(int deviceId, IEnumerable<RootCause> rootCauses)
    {
        var ancestors = await _topologyClient.GetAncestorsAsync(deviceId);
        var ancestorIds = ancestors.Select(a => a.DeviceId).ToHashSet();
        
        var rootCauseDict = rootCauses.ToDictionary(rc => rc.DeviceId);
        
        // Find if any ancestor is an active root cause
        foreach (var ancestorId in ancestorIds)
        {
            if (rootCauseDict.TryGetValue(ancestorId, out var rc))
            {
                return rc;
            }
        }

        return null;
    }

    private async Task<HashSet<int>> GetDownstreamDeviceIdsAsync(int rootDeviceId)
    {
        var descendants = await _topologyClient.GetDescendantsAsync(rootDeviceId);
        return descendants.Select(d => d.DeviceId).ToHashSet();
    }

    private async Task RebuildImpactedDevicesAsync(int rootCauseId, int rootDeviceId, HashSet<int> impactedDeviceIds)
    {
        await _impactedDeviceRepository.DeleteByRootCauseAsync(rootCauseId);

        if (impactedDeviceIds.Count == 0) return;

        var impactedDevices = impactedDeviceIds.Select(id => new ImpactedDevice
        {
            RootCauseId = rootCauseId,
            DeviceId = id,
            ImpactType = "DOWNSTREAM"
        });

        await _impactedDeviceRepository.CreateRangeAsync(impactedDevices);
    }
}
