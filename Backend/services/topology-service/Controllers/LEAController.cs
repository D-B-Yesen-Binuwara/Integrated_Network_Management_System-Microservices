using Microsoft.AspNetCore.Mvc;
using topology_service.Entities;
using topology_service.Services;

namespace topology_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LEAController : ControllerBase
{
    private readonly ILEAService _service;

    public LEAController(ILEAService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var e = await _service.GetByIdAsync(id);
        if (e == null) return NotFound();
        return Ok(e);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] LEA lea)
    {
        return Ok(await _service.CreateAsync(lea));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LEA lea)
    {
        return Ok(await _service.UpdateAsync(id, lea));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok("Deleted");
    }
}
