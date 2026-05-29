using INMS.Identity.Domain.Entities;

namespace INMS.Identity.Domain.Interfaces;

public interface IUserAreaAssignmentRepository
{
    Task<List<UserAreaAssignment>> GetAllByUserId(int userId);
    Task AssignArea(UserAreaAssignment assignment);
    Task RemoveAssignmentsByUserId(int userId);
}
