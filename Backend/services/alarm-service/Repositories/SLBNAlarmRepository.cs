using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Entities;

namespace alarm_service.Repositories;

public class SLBNAlarmRepository : ISLBNAlarmRepository
{
    private readonly AlarmDbContext _context;

    public SLBNAlarmRepository(AlarmDbContext context)
    {
        _context = context;
    }

    public async Task<SLBNAlarm?> GetByIdAsync(int id)
    {
        return await _context.SLBNAlarms.FindAsync(id);
    }

    public async Task<List<SLBNAlarm>> GetAllAsync()
    {
        return await _context.SLBNAlarms.ToListAsync();
    }

    public async Task<List<SLBNAlarm>> GetByDeviceIdAsync(int deviceId)
    {
        return await _context.SLBNAlarms
            .Where(a => a.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<SLBNAlarm> AddAsync(SLBNAlarm alarm)
    {
        await _context.SLBNAlarms.AddAsync(alarm);
        await _context.SaveChangesAsync();
        return alarm;
    }

    public async Task<SLBNAlarm> UpdateAsync(SLBNAlarm alarm)
    {
        _context.SLBNAlarms.Update(alarm);
        await _context.SaveChangesAsync();
        return alarm;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alarm = await _context.SLBNAlarms.FindAsync(id);
        if (alarm == null) return false;

        _context.SLBNAlarms.Remove(alarm);
        await _context.SaveChangesAsync();
        return true;
    }
}

