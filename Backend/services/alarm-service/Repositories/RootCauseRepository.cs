using alarm_service.Data;
using alarm_service.Entities;
using alarm_service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Repositories;

public class RootCauseRepository : IRootCauseRepository
{
    private readonly AlarmDbContext _context;

    public RootCauseRepository(AlarmDbContext context)
    {
        _context = context;
    }

    public async Task<RootCause?> GetByIdAsync(int id)
    {
        return await _context.RootCauses.FindAsync(id);
    }

    public async Task<RootCause?> GetByDeviceIdAsync(int deviceId)
    {
        return await _context.RootCauses
            .FirstOrDefaultAsync(r => r.DeviceId == deviceId);
    }

    public async Task<IEnumerable<RootCause>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.RootCauses.ToListAsync(cancellationToken);
    }

    public async Task<RootCause> CreateAsync(RootCause rootCause)
    {
        _context.RootCauses.Add(rootCause);
        await _context.SaveChangesAsync();
        return rootCause;
    }

    public async Task DeleteAsync(int id)
    {
        var rootCause = await _context.RootCauses.FindAsync(id);
        if (rootCause != null)
        {
            _context.RootCauses.Remove(rootCause);
            await _context.SaveChangesAsync();
        }
    }
}
