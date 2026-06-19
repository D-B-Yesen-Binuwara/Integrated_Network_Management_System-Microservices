using Microsoft.AspNetCore.Mvc;
using INMS.Identity.Application.Interfaces;
using INMS.Identity.Domain.Entities;

namespace INMS.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _service;

    public RoleController(IRoleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Role role)
    {
        await _service.CreateAsync(role);
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = await _service.GetByIdAsync(id);
        if (role == null) return NotFound();
        return Ok(role);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Role role)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        var updated = await _service.UpdateAsync(id, role);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existing = await _service.GetByIdAsync(id);
        if (existing == null) return NotFound();
        await _service.DeleteAsync(id);
        return Ok();
    }
}
