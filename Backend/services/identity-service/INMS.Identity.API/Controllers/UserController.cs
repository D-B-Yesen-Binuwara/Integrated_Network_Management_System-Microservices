using Microsoft.AspNetCore.Mvc;
using INMS.Identity.Application.Interfaces;
using INMS.Identity.Application.DTOs;
using INMS.Identity.Application.Services;

namespace INMS.Identity.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly UserAreaAssignmentService _areaService;

    public UserController(IUserService service, UserAreaAssignmentService areaService)
    {
        _service = service;
        _areaService = areaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _service.GetAll());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id) => Ok(await _service.GetById(id));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        await _service.CreateFromDto(dto);
        return Ok(new { message = "User created" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.Delete(id);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var existing = await _service.GetById(id);
        if (existing == null) return NotFound();

        await _service.Update(id, dto.Username, dto.RoleId);
        return Ok(new { message = "User updated" });
    }

    [HttpGet("{id}/areas")]
    public async Task<IActionResult> GetAreas(int id)
    {
        var existing = await _service.GetById(id);
        if (existing == null) return NotFound();

        var areas = await _areaService.GetUserAreas(id);
        return Ok(areas);
    }

    [HttpPost("{id}/areas")]
    public async Task<IActionResult> AssignArea(int id, [FromBody] AssignAreaDto dto)
    {
        var existing = await _service.GetById(id);
        if (existing == null) return NotFound();

        try
        {
            await _areaService.AssignArea(id, dto.AreaType, dto.AreaId);
            return Ok(new { message = "Area assigned" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id}/areas")]
    public async Task<IActionResult> ReplaceAreas(int id, [FromBody] UpdateUserAreasDto dto)
    {
        var existing = await _service.GetById(id);
        if (existing == null) return NotFound();

        try
        {
            await _areaService.ReplaceUserAreas(id, dto.Assignments);
            return Ok(new { message = "User areas replaced" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
