using Microsoft.AspNetCore.Mvc;
using topology_service.DTOs;
using topology_service.Services;

namespace topology_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceController : ControllerBase
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceDto>>> GetAll()
    {
        var devices = await _deviceService.GetAllAsync();
        return Ok(devices);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DeviceDto>> GetById(int id)
    {
        var device = await _deviceService.GetByIdAsync(id);
        return device == null ? NotFound() : Ok(device);
    }

    [HttpPost]
    public async Task<ActionResult<DeviceDto>> Create([FromBody] CreateDeviceDto dto)
    {
        var created = await _deviceService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.DeviceId }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DeviceDto>> Update(int id, [FromBody] UpdateDeviceDto dto)
    {
        var updated = await _deviceService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        return await _deviceService.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
