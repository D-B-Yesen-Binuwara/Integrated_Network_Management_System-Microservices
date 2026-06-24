using Microsoft.EntityFrameworkCore;
using topology_service.Data;
using topology_service.Entities;

namespace topology_service.Repositories;

public class DeviceLinkRepository : IDeviceLinkRepository
{
    private readonly TopologyDbContext _context;

    public DeviceLinkRepository(TopologyDbContext context)
    {
        _context = context;
    }

    public async Task<DeviceLink> AddAsync(DeviceLink link)
    {
        await _context.DeviceLinks.AddAsync(link);
        await _context.SaveChangesAsync();
        return link;
    }

    public async Task<List<DeviceLink>> GetAllAsync()
    {
        return await _context.DeviceLinks.AsNoTracking().ToListAsync();
    }

    public async Task<DeviceLink?> GetByIdAsync(int id)
    {
        return await _context.DeviceLinks.AsNoTracking().FirstOrDefaultAsync(link => link.LinkId == id);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _context.DeviceLinks.FirstOrDefaultAsync(link => link.LinkId == id);
        if (existing == null)
        {
            return false;
        }

        _context.DeviceLinks.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<DeviceLink>> GetChildLinksAsync(int parentDeviceId)
    {
        return await _context.DeviceLinks
            .AsNoTracking()
            .Include(l => l.ChildDevice)
            .Where(l => l.ParentDeviceId == parentDeviceId)
            .ToListAsync();
    }

    public async Task<List<DeviceLink>> GetParentLinksAsync(int childDeviceId)
    {
        return await _context.DeviceLinks
            .AsNoTracking()
            .Include(l => l.ParentDevice)
            .Where(l => l.ChildDeviceId == childDeviceId)
            .ToListAsync();
    }
}
