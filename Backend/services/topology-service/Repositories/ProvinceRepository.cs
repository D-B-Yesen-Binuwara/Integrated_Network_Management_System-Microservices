using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Entities;

namespace topology_service.Repositories;

public class ProvinceRepository : IProvinceRepository
{
    private readonly TopologyDbContext _context;

    public ProvinceRepository(TopologyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Province>> GetAllAsync()
    {
        return await _context.Set<Province>()
            .Include(p => p.Region)
            .AsNoTracking()
            .OrderBy(p => p.ProvinceId)
            .ToListAsync();
    }

    public async Task<Province?> GetByIdAsync(int id)
    {
        return await _context.Set<Province>()
            .Include(p => p.Region)
            .FirstOrDefaultAsync(p => p.ProvinceId == id);
    }

    public async Task<Province> AddAsync(Province province)
    {
        await _context.Set<Province>().AddAsync(province);
        await _context.SaveChangesAsync();
        return province;
    }

    public async Task<Province> UpdateAsync(Province province)
    {
        _context.Set<Province>().Update(province);
        await _context.SaveChangesAsync();
        return province;
    }

    public async Task DeleteAsync(int id)
    {
        var province = await _context.Set<Province>().FindAsync(id);
        if (province == null) throw new Exception("Province not found");
        _context.Set<Province>().Remove(province);
        await _context.SaveChangesAsync();
    }
}
