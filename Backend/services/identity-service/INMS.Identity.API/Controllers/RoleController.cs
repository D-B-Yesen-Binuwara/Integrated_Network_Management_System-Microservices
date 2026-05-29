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
}
