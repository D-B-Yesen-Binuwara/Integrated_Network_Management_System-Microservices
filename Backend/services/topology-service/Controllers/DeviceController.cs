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
    public ActionResult<IEnumerable<DeviceDto>> GetAll()
    {
        return Ok(_deviceService.GetAll());
    }

    [HttpGet("{id:int}")]
    public ActionResult<DeviceDto> GetById(int id)
    {
        var device = _deviceService.GetById(id);
        return device == null ? NotFound() : Ok(device);
    }

    [HttpPost]
    public ActionResult<DeviceDto> Create([FromBody] CreateDeviceDto dto)
    {
        var created = _deviceService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.DeviceId }, created);
    }

    [HttpPut("{id:int}")]
    public ActionResult<DeviceDto> Update(int id, [FromBody] UpdateDeviceDto dto)
    {
        var updated = _deviceService.Update(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        return _deviceService.Delete(id) ? NoContent() : NotFound();
    }
}
