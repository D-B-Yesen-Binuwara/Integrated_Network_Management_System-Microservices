using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Entities;

namespace topology_service.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly TopologyDbContext _context;

    public RegionRepository(TopologyDbContext context)
    {
        _context = context;
    }

    public async Task<List<Region>> GetAllAsync()
    {
        return await _context.Set<Region>().AsNoTracking().OrderBy(r => r.RegionId).ToListAsync();
    }

    public async Task<Region?> GetByIdAsync(int id)
    {
        return await _context.Set<Region>().FindAsync(id);
    }

    public async Task<Region> AddAsync(Region region)
    {
        await _context.Set<Region>().AddAsync(region);
        await _context.SaveChangesAsync();
        return region;
    }

    public async Task<Region> UpdateAsync(Region region)
    {
        _context.Set<Region>().Update(region);
        await _context.SaveChangesAsync();
        return region;
    }

    public async Task DeleteAsync(int id)
    {
        var region = await _context.Set<Region>().FindAsync(id);
        if (region == null) throw new Exception("Region not found");
        _context.Set<Region>().Remove(region);
        await _context.SaveChangesAsync();
    }
}
