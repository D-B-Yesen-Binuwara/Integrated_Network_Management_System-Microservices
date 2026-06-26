using Microsoft.AspNetCore.Mvc;
using alarm_service.DTOs;
using alarm_service.Services.Implement;


namespace alarm_service.Controllers;

[ApiController]
[Route("api/cea-alarms")]
public class CEAAlarmController : ControllerBase
{
    private readonly ICEAAlarmService _alarmService;

    public CEAAlarmController(ICEAAlarmService alarmService)
    {
        _alarmService = alarmService;
    }

    [HttpPost]
    public async Task<ActionResult<CEAAlarmResponseDto>> Create([FromBody] CreateCEAAlarmRequestDto dto)
    {
        var created = await _alarmService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.CEAAlarmId }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CEAAlarmResponseDto>>> GetAll()
    {
        var alarms = await _alarmService.GetAllAsync();
        return Ok(alarms);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CEAAlarmResponseDto>> GetById(int id)
    {
        var alarm = await _alarmService.GetByIdAsync(id);
        return alarm == null ? NotFound() : Ok(alarm);
    }

    [HttpGet("filtered")]
    public async Task<ActionResult<List<CEAAlarmListDto>>> GetFiltered(
        [FromQuery] bool? isActive = null,
        [FromQuery] DateTime? dateFrom = null,
        [FromQuery] DateTime? dateTo = null,
        [FromQuery] int? deviceId = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? order = "desc")
    {
        var queryParams = new CEAAlarmQueryParams(isActive, dateFrom, dateTo, deviceId, sortBy, order);
        var result = await _alarmService.GetFilteredAsync(queryParams);
        return Ok(result);
    }

    [HttpGet("active")]
    public async Task<ActionResult<List<CEAAlarmResponseDto>>> GetActive()
    {
        var alarms = await _alarmService.GetActiveAsync();
        return Ok(alarms);
    }

    [HttpGet("device/{deviceId:int}")]
    public async Task<ActionResult<List<CEAAlarmResponseDto>>> GetByDeviceId(int deviceId)
    {
        var alarms = await _alarmService.GetByDeviceIdAsync(deviceId);
        return Ok(alarms);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CEAAlarmResponseDto>> Update(int id, [FromBody] UpdateCEAAlarmRequestDto dto)
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

