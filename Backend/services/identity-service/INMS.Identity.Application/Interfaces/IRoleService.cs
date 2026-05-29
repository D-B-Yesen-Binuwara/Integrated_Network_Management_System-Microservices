using INMS.Identity.Domain.Entities;

namespace INMS.Identity.Application.Interfaces;

public interface IRoleService
{
    Task<List<Role>> GetAllAsync();
    Task<Role?> GetByIdAsync(int id);
    Task<Role> CreateAsync(Role role);
    Task<Role> UpdateAsync(int id, Role role);
    Task DeleteAsync(int id);
}
