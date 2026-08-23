using alarm_service.Correlation.Engine;
using alarm_service.Correlation.Models;
using alarm_service.Data;
using alarm_service.DTOs.Requests;
using alarm_service.DTOs.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Controllers;

[ApiController]
[Route("api/correlation")]
public class CorrelationController : ControllerBase
{
    private readonly ICorrelationEngine _correlationEngine;
    private readonly AlarmDbContext _context;

    public CorrelationController(ICorrelationEngine correlationEngine, AlarmDbContext context)
    {
        _correlationEngine = correlationEngine;
        _context = context;
    }

    [HttpPost("evaluate")]
    public async Task<ActionResult<CorrelationResult>> Evaluate(
        [FromBody] EvaluateCorrelationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.AlarmId <= 0 || request.DeviceId <= 0 ||
            string.IsNullOrWhiteSpace(request.AlarmType) ||
            string.IsNullOrWhiteSpace(request.DeviceType))
        {
            return BadRequest("AlarmId, DeviceId, AlarmType, and DeviceType are required.");
        }

        var result = await _correlationEngine.EvaluateAsync(new CorrelationContext
        {
            AlarmId = request.AlarmId,
            DeviceId = request.DeviceId,
            AlarmType = request.AlarmType,
            DeviceType = request.DeviceType,
            RaisedTime = request.RaisedTime?.ToUniversalTime() ?? DateTime.UtcNow
        }, cancellationToken);

        return Ok(result);
    }

    [HttpGet("faults")]
    public async Task<ActionResult<IEnumerable<CorrelatedFaultResponse>>> GetFaults(CancellationToken cancellationToken)
    {
        var faults = await _context.CorrelatedFaults
            .AsNoTracking()
            .Include(fault => fault.SuppressedAlarms)
            .OrderByDescending(fault => fault.StartedAt)
            .ToListAsync(cancellationToken);

        return Ok(faults.Select(ToResponse));
    }

    [HttpGet("faults/{id:int}")]
    public async Task<ActionResult<CorrelatedFaultResponse>> GetFault(int id, CancellationToken cancellationToken)
    {
        var fault = await _context.CorrelatedFaults
            .AsNoTracking()
            .Include(item => item.SuppressedAlarms)
            .FirstOrDefaultAsync(item => item.CorrelatedFaultId == id, cancellationToken);

        return fault is null ? NotFound() : Ok(ToResponse(fault));
    }

    [HttpGet("impacted-devices")]
    public async Task<ActionResult> GetImpactedDevices([FromQuery] int? rootCauseId, CancellationToken cancellationToken)
    {
        var query = _context.ImpactedDevices.AsNoTracking().AsQueryable();
        if (rootCauseId.HasValue) query = query.Where(item => item.RootCauseId == rootCauseId.Value);

        var results = await query
            .OrderByDescending(item => item.CreatedAt)
            .Select(item => new
            {
                item.ImpactedDeviceId,
                item.RootCauseId,
                item.DeviceId,
                item.DeviceType,
                item.ImpactType,
                item.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return Ok(results);
    }

    private static CorrelatedFaultResponse ToResponse(Entities.CorrelatedFault fault) => new()
    {
        CorrelatedFaultId = fault.CorrelatedFaultId,
        RootCauseId = fault.RootCauseId,
        CorrelationRuleName = fault.CorrelationRuleName,
        SourceDeviceId = fault.SourceDeviceId,
        SourceDeviceType = fault.SourceDeviceType,
        SourceAlarmId = fault.SourceAlarmId,
        SourceAlarmType = fault.SourceAlarmType,
        StartedAt = fault.StartedAt,
        EndedAt = fault.EndedAt,
        Status = fault.Status,
        ConfidenceScore = fault.ConfidenceScore,
        SuppressedAlarms = fault.SuppressedAlarms.Select(alarm => new SuppressedAlarmResponse
        {
            AlarmId = alarm.AlarmId,
            DeviceId = alarm.DeviceId,
            DeviceType = alarm.DeviceType,
            AlarmType = alarm.AlarmType,
            RaisedTime = alarm.RaisedTime
        }).ToList()
    };
}
