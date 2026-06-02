using Microsoft.AspNetCore.Mvc;
using topology_service.DTOs;
using topology_service.Services;

namespace topology_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeviceLinkController : ControllerBase
{
    private readonly IDeviceLinkService _service;

    public DeviceLinkController(IDeviceLinkService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<ActionResult<DeviceLinkDto>> CreateLink([FromBody] CreateDeviceLinkDto request)
    {
        try
        {
            var link = await _service.CreateLinkAsync(request);
            // Since we don't have a GetById endpoint for link yet, we can return Ok or Created.
            // Let's return Ok(link) or register a GetById endpoint if desired. Returning Ok(link) matches the sample controller.
            return Ok(link);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviceLinkDto>>> GetAll()
    {
        var links = await _service.GetAllLinksAsync();
        return Ok(links);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteLinkAsync(id);
        return success ? NoContent() : NotFound();
    }
}
