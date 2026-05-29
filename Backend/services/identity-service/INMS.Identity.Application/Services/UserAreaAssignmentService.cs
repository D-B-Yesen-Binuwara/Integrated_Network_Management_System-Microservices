using INMS.Identity.Domain.Interfaces;

namespace INMS.Identity.Application.Services;

public class UserAreaAssignmentService
{
    private readonly IUserAreaAssignmentRepository _repository;

    public UserAreaAssignmentService(IUserAreaAssignmentRepository repository)
    {
        _repository = repository;
    }

    public async Task AssignArea(int userId, string areaType, Guid areaId)
    {
        if (areaType != "Region" && areaType != "Province" && areaType != "LEA")
            throw new Exception("Invalid AreaType");

        await _repository.AssignArea(new INMS.Identity.Domain.Entities.UserAreaAssignment
        {
            UserId = userId,
            AreaType = areaType,
            AreaId = areaId
        });
    }
}
