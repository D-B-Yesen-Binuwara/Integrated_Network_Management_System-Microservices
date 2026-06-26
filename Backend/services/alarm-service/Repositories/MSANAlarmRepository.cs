using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Entities;
using alarm_service.Repositories.Interfaces;

namespace alarm_service.Repositories;

public class MSANAlarmRepository : IMSANAlarmRepository
{
    private readonly AlarmDbContext _context;

    public MSANAlarmRepository(AlarmDbContext context)
    {
        _context = context;
    }

    public async Task<MSANAlarm?> GetByIdAsync(int id)
    {
        return await _context.MSANAlarms.FindAsync(id);
    }

    public async Task<List<MSANAlarm>> GetAllAsync()
    {
        return await _context.MSANAlarms.ToListAsync();
    }

    public async Task<List<MSANAlarm>> GetByDeviceIdAsync(int deviceId)
    {
        return await _context.MSANAlarms
            .Where(a => a.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<MSANAlarm> AddAsync(MSANAlarm alarm)
    {
        await _context.MSANAlarms.AddAsync(alarm);
        await _context.SaveChangesAsync();
        return alarm;
    }

    public async Task<MSANAlarm> UpdateAsync(MSANAlarm alarm)
    {
        _context.MSANAlarms.Update(alarm);
        await _context.SaveChangesAsync();
        return alarm;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alarm = await _context.MSANAlarms.FindAsync(id);
        if (alarm == null) return false;

        _context.MSANAlarms.Remove(alarm);
        await _context.SaveChangesAsync();
        return true;
    }
}

