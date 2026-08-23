using alarm_service.DTOs.Requests;
using alarm_service.Correlation.Engine;
using alarm_service.Correlation.Models;
using alarm_service.Services.Implement;
using alarm_service.Repositories.Interfaces;

using Microsoft.AspNetCore.Mvc;

namespace alarm_service.Controllers;

[ApiController]
[Route("api/impact-analysis")]
public class ImpactAnalysisController : ControllerBase
{
    private readonly IImpactAnalysisService _impactAnalysisService;
    private readonly ICorrelationEngine _correlationEngine;

    public ImpactAnalysisController(IImpactAnalysisService impactAnalysisService, ICorrelationEngine correlationEngine)
    {
        _impactAnalysisService = impactAnalysisService;
        _correlationEngine = correlationEngine;
    }

    [HttpPost("analyze")]
    public async Task<IActionResult> Analyze([FromBody] AnalyzeImpactRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AlarmType) || string.IsNullOrWhiteSpace(request.DeviceType))
            return BadRequest("AlarmType and DeviceType are required for rule-driven correlation.");

        var result = await _correlationEngine.EvaluateAsync(new CorrelationContext
        {
            AlarmId = request.AlarmId,
            DeviceId = request.DeviceId,
            AlarmType = request.AlarmType,
            DeviceType = request.DeviceType,
            RaisedTime = request.RaisedTime?.ToUniversalTime() ?? DateTime.UtcNow
        });
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
