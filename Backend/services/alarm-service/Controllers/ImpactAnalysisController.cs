using alarm_service.DTOs.Requests;
using alarm_service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace alarm_service.Controllers;

[ApiController]
[Route("api/impact-analysis")]
public class ImpactAnalysisController : ControllerBase
{
    private readonly IImpactAnalysisService _impactAnalysisService;

    public ImpactAnalysisController(IImpactAnalysisService impactAnalysisService)
    {
        _impactAnalysisService = impactAnalysisService;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] AnalyzeImpactRequest request)
    {
        var result = await _impactAnalysisService.AnalyzeFailureAsync(request.DeviceId, request.AlarmId);
        return Ok(result);
    }

    [HttpGet("root-causes")]
    public async Task<IActionResult> GetRootCauses()
    {
        var result = await _impactAnalysisService.GetRootCausesAsync();
        return Ok(result);
    }

    [HttpGet("root-causes/{rootCauseId}/impacted-devices")]
    public async Task<IActionResult> GetImpactedDevices(int rootCauseId)
    {
        var result = await _impactAnalysisService.GetImpactedDevicesAsync(rootCauseId);
        return Ok(result);
    }

    [HttpPost("clear/{deviceId}")]
    public async Task<IActionResult> ClearRootCause(int deviceId)
    {
        await _impactAnalysisService.ClearRootCauseAsync(deviceId);
        return Ok();
    }

    [HttpPost("rebuild/{deviceId}")]
    public async Task<IActionResult> RebuildImpact(int deviceId)
    {
        await _impactAnalysisService.RebuildImpactAsync(deviceId);
        return Ok();
    }
}
