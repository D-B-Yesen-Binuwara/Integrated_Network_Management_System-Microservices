using Microsoft.AspNetCore.Mvc;
using alarm_service.DTOs;
using alarm_service.Services.Implement;

namespace alarm_service.Controllers;

[ApiController]
[Route("api/slbn-alarms")]
public class SLBNAlarmController : ControllerBase
{
    private readonly ISLBNAlarmService _alarmService;

    public SLBNAlarmController(ISLBNAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpPost]
    public async Task<ActionResult<SLBNAlarmResponseDto>> Create([FromBody] CreateSLBNAlarmRequestDto dto)
    {
        var created = await _alarmService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.SLBNAlarmId }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SLBNAlarmResponseDto>>> GetAll()
    {
        var alarms = await _alarmService.GetAllAsync();
        return Ok(alarms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SLBNAlarmResponseDto>> GetById(int id)
    {
        var alarm = await _alarmService.GetByIdAsync(id);
        return alarm == null ? NotFound() : Ok(alarm);
    }

    [HttpGet("filtered")]
    public async Task<ActionResult<List<SLBNAlarmListDto>>> GetFiltered(
        [FromQuery] bool? isActive = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int? deviceId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? order = "desc")
    {
        var queryParams = new SLBNAlarmQueryParams(isActive, dateFrom, dateTo, deviceId, sortBy, order);
        var result = await _alarmService.GetFilteredAsync(queryParams);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<SLBNAlarmResponseDto>>> GetActive()
    {
        var alarms = await _alarmService.GetActiveAsync();
        return Ok(alarms);
    }

    [HttpGet("device/{deviceId:int}")]
    public async Task<ActionResult<List<SLBNAlarmResponseDto>>> GetByDeviceId(int deviceId)
    {
        var alarms = await _alarmService.GetByDeviceIdAsync(deviceId);
        return Ok(alarms);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SLBNAlarmResponseDto>> Update(int id, [FromBody] UpdateSLBNAlarmRequestDto dto)
    {
        var updated = await _alarmService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _alarmService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}

