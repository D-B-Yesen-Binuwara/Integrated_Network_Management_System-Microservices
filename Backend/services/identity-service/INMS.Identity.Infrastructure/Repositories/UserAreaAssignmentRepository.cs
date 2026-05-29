using INMS.Identity.Domain.Entities;
using INMS.Identity.Domain.Interfaces;
using INMS.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace INMS.Identity.Infrastructure.Repositories;

public class UserAreaAssignmentRepository : IUserAreaAssignmentRepository
{
    private readonly IdentityDbContext _context;

    public UserAreaAssignmentRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<List<UserAreaAssignment>> GetAllByUserId(int userId)
    {
        return await _context.UserAreaAssignments.Where(u => u.UserId == userId).ToListAsync();
    }

    public async Task AssignArea(UserAreaAssignment assignment)
    {
        _context.UserAreaAssignments.Add(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveAssignmentsByUserId(int userId)
    {
        var assignments = await _context.UserAreaAssignments.Where(a => a.UserId == userId).ToListAsync();
        _context.UserAreaAssignments.RemoveRange(assignments);
        await _context.SaveChangesAsync();
    }
}
