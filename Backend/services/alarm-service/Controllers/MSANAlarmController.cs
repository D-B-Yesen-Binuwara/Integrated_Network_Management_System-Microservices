using Microsoft.AspNetCore.Mvc;
using alarm_service.DTOs;
using alarm_service.Services;

namespace alarm_service.Controllers;

[ApiController]
[Route("api/msan-alarms")]
public class MSANAlarmController : ControllerBase
{
    private readonly IMSANAlarmService _alarmService;

    public MSANAlarmController(IMSANAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpPost]
    public async Task<ActionResult<MSANAlarmResponseDto>> Create([FromBody] CreateMSANAlarmRequestDto dto)
    {
        var created = await _alarmService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.MSANAlarmId }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MSANAlarmResponseDto>>> GetAll()
    {
        var alarms = await _alarmService.GetAllAsync();
        return Ok(alarms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MSANAlarmResponseDto>> GetById(int id)
    {
        var alarm = await _alarmService.GetByIdAsync(id);
        return alarm == null ? NotFound() : Ok(alarm);
    }

    [HttpGet("filtered")]
    public async Task<ActionResult<List<MSANAlarmListDto>>> GetFiltered(
        [FromQuery] bool? isActive = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int? deviceId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? order = "desc")
    {
        var queryParams = new MSANAlarmQueryParams(isActive, dateFrom, dateTo, deviceId, sortBy, order);
        var result = await _alarmService.GetFilteredAsync(queryParams);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<MSANAlarmResponseDto>>> GetActive()
    {
        var alarms = await _alarmService.GetActiveAsync();
        return Ok(alarms);
    }

    [HttpGet("device/{deviceId:int}")]
    public async Task<ActionResult<List<MSANAlarmResponseDto>>> GetByDeviceId(int deviceId)
    {
        var alarms = await _alarmService.GetByDeviceIdAsync(deviceId);
        return Ok(alarms);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<MSANAlarmResponseDto>> Update(int id, [FromBody] UpdateMSANAlarmRequestDto dto)
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

