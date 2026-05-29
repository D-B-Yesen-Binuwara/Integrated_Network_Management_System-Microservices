using INMS.Identity.Application.Interfaces;
using INMS.Identity.Domain.Entities;
using INMS.Identity.Domain.Interfaces;

namespace INMS.Identity.Application.Services;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;

    public RoleService(IRoleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Role>> GetAllAsync() => await _repository.GetAll();

    public async Task<Role?> GetByIdAsync(int id) => await _repository.GetById(id);

    public async Task<Role> CreateAsync(Role role)
    {
        await _repository.Create(role);
        return role;
    }

    public async Task<Role> UpdateAsync(int id, Role role)
    {
        var existing = await _repository.GetById(id);
        if (existing == null) throw new Exception("Role not found");
        existing.RoleName = role.RoleName;
        existing.Description = role.Description;
        await _repository.Update(existing);
        return existing;
    }

    public async Task DeleteAsync(int id) => await _repository.Delete(id);
}
