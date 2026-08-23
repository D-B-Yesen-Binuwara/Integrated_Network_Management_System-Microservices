using alarm_service.DTOs.Responses;

namespace alarm_service.Services.Implement;

public interface IImpactAnalysisService
{
    Task<AnalyzeImpactResponse> AnalyzeFailureAsync(int deviceId, int alarmId, string alarmType = "UNKNOWN", string deviceType = "");
    Task<IEnumerable<RootCauseResponse>> GetRootCausesAsync();
    Task<IEnumerable<ImpactedDeviceResponse>> GetImpactedDevicesAsync(int rootCauseId);
    Task ClearRootCauseAsync(int deviceId);
    Task ReevaluateDownstreamFailuresAsync(IEnumerable<int> previouslyImpactedDeviceIds);
    Task RebuildImpactAsync(int deviceId);
}
