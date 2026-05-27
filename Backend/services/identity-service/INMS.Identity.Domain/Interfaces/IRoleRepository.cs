using INMS.Identity.Domain.Entities;

namespace INMS.Identity.Domain.Interfaces;

public interface IRoleRepository
{
    Task<List<Role>> GetAll();
    Task<Role?> GetById(int id);
    Task Create(Role role);
    Task Update(Role role);
    Task Delete(int id);
}
