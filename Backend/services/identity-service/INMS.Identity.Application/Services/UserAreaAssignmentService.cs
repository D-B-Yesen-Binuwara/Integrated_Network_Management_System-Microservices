using INMS.Identity.Domain.Interfaces;

namespace INMS.Identity.Application.Services;

using INMS.Identity.Application.DTOs;

public class UserAreaAssignmentService
{
    private readonly IUserAreaAssignmentRepository _repository;
    private readonly INMS.Identity.Application.Interfaces.IAreaValidator _areaValidator;

    public UserAreaAssignmentService(IUserAreaAssignmentRepository repository, INMS.Identity.Application.Interfaces.IAreaValidator areaValidator)
    {
        _repository = repository;
        _areaValidator = areaValidator;
    }

    public async Task AssignArea(int userId, string areaType, Guid areaId, string? regionCode = null, string? provinceCode = null, string? leaCode = null)
    {
        if (areaType != "Region" && areaType != "Province" && areaType != "LEA")
            throw new Exception("Invalid AreaType");

        if (areaId == Guid.Empty) throw new Exception("AreaId cannot be empty");

        // Validate existence via validator
        if (!await _areaValidator.AreaExists(areaType, areaId))
            throw new Exception("Referenced area does not exist");

        // Prevent duplicate assignments for the same user
        var existing = await _repository.GetAllByUserId(userId);
        if (existing.Any(a => a.AreaType == areaType && a.AreaId == areaId))
            throw new Exception("Duplicate area assignment");

        await _repository.AssignArea(new INMS.Identity.Domain.Entities.UserAreaAssignment
        {
            UserId = userId,
            AreaType = areaType,
            AreaId = areaId,
            RegionCode = regionCode,
            ProvinceCode = provinceCode,
            LEACode = leaCode
        });
    }

    public async Task<List<UserAreaAssignmentDto>> GetUserAreas(int userId)
    {
        var assignments = await _repository.GetAllByUserId(userId);
        return assignments.Select(a => new UserAreaAssignmentDto(a.AssignmentId, a.AreaType, a.AreaId, a.RegionCode, a.ProvinceCode, a.LEACode)).ToList();
    }

    public async Task ReplaceUserAreas(int userId, List<AssignAreaDto> assignments)
    {
        // Remove existing assignments
        await _repository.RemoveAssignmentsByUserId(userId);

        // Add new assignments
        if (assignments == null) return;

        foreach (var a in assignments)
        {
            // reuse validation from AssignArea
            if (a == null) continue;
            await AssignArea(userId, a.AreaType, a.AreaId, a.RegionCode, a.ProvinceCode, a.LEACode);
        }
    }
}
