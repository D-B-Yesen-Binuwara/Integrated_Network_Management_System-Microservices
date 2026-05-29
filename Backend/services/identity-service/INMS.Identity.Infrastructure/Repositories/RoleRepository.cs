using INMS.Identity.Domain.Entities;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace INMS.Identity.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly IdentityDbContext _context;

    public RoleRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<Role>> GetAll()
    {
        return await _context.Roles.ToListAsync();
    }

    public async Task<Role?> GetById(int id)
    {
        return await _context.Roles.FindAsync(id);
    }

    public async Task Create(Role role)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
    }

    public async Task Update(Role role)
    {
        _context.Roles.Update(role);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }
}
