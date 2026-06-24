using alarm_service.DTOs.Responses;

namespace alarm_service.Interfaces;

public interface IImpactAnalysisService
{
    Task<AnalyzeImpactResponse> AnalyzeFailureAsync(int deviceId, int alarmId);
    Task<IEnumerable<RootCauseResponse>> GetRootCausesAsync();
    Task<IEnumerable<ImpactedDeviceResponse>> GetImpactedDevicesAsync(int rootCauseId);
    Task ClearRootCauseAsync(int deviceId);
    Task ReevaluateDownstreamFailuresAsync(IEnumerable<int> previouslyImpactedDeviceIds);
    Task RebuildImpactAsync(int deviceId);
}
