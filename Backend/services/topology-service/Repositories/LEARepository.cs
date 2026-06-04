using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Entities;

namespace topology_service.Repositories;

public class LEARepository : ILEARepository
{
    private readonly TopologyDbContext _context;

    public LEARepository(TopologyDbContext context)
    {
        _context = context;
    }

    public async Task<List<LEA>> GetAllAsync()
    {
        return await _context.Set<LEA>()
            .Include(l => l.Province)
            .AsNoTracking()
            .OrderBy(l => l.LEAId)
            .ToListAsync();
    }

    public async Task<LEA?> GetByIdAsync(int id)
    {
        return await _context.Set<LEA>()
            .Include(l => l.Province)
            .FirstOrDefaultAsync(l => l.LEAId == id);
    }

    public async Task<LEA> AddAsync(LEA lea)
    {
        await _context.Set<LEA>().AddAsync(lea);
        await _context.SaveChangesAsync();
        return lea;
    }

    public async Task<LEA> UpdateAsync(LEA lea)
    {
        _context.Set<LEA>().Update(lea);
        await _context.SaveChangesAsync();
        return lea;
    }

    public async Task DeleteAsync(int id)
    {
        var lea = await _context.Set<LEA>().FindAsync(id);
        if (lea == null) throw new Exception("LEA not found");
        _context.Set<LEA>().Remove(lea);
        await _context.SaveChangesAsync();
    }
}
