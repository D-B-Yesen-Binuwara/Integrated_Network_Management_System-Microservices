using Microsoft.AspNetCore.Mvc;
using topology_service.Entities;
using topology_service.Services;

namespace topology_service.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProvinceController : ControllerBase
{
    private readonly IProvinceService _service;

    public ProvinceController(IProvinceService service)
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
        var p = await _service.GetByIdAsync(id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Province province)
    {
        return Ok(await _service.CreateAsync(province));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Province province)
    {
        return Ok(await _service.UpdateAsync(id, province));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return Ok("Deleted");
    }
}
