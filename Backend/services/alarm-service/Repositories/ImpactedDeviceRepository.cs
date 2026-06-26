using alarm_service.Data;
using alarm_service.Entities;
using alarm_service.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Repositories;

public class ImpactedDeviceRepository : IImpactedDeviceRepository
{
    private readonly AlarmDbContext _context;

    public ImpactedDeviceRepository(AlarmDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ImpactedDevice>> GetByRootCauseIdAsync(int rootCauseId)
    {
        return await _context.ImpactedDevices
            .Where(i => i.RootCauseId == rootCauseId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ImpactedDevice>> GetByDeviceIdAsync(int deviceId)
    {
        return await _context.ImpactedDevices
            .Where(i => i.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<ImpactedDevice> CreateAsync(ImpactedDevice impactedDevice)
    {
        _context.ImpactedDevices.Add(impactedDevice);
        await _context.SaveChangesAsync();
        return impactedDevice;
    }

    public async Task CreateRangeAsync(IEnumerable<ImpactedDevice> impactedDevices)
    {
        await _context.ImpactedDevices.AddRangeAsync(impactedDevices);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var impactedDevice = await _context.ImpactedDevices.FindAsync(id);
        if (impactedDevice != null)
        {
            _context.ImpactedDevices.Remove(impactedDevice);
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteByRootCauseAsync(int rootCauseId)
    {
        var devices = await _context.ImpactedDevices
            .Where(i => i.RootCauseId == rootCauseId)
            .ToListAsync();

        if (devices.Any())
        {
            _context.ImpactedDevices.RemoveRange(devices);
            await _context.SaveChangesAsync();
        }
    }
}
