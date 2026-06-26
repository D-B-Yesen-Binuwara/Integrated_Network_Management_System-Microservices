using Microsoft.EntityFrameworkCore;
using alarm_service.Data;
using alarm_service.Entities;
using alarm_service.Repositories.Interfaces;

namespace alarm_service.Repositories;

public class CEAAlarmRepository : ICEAAlarmRepository
{
    private readonly AlarmDbContext _context;

    public CEAAlarmRepository(AlarmDbContext context)
    {
        _context = context;
    }

    public async Task<CEAAlarm?> GetByIdAsync(int id)
    {
        return await _context.CEAAlarms.FindAsync(id);
    }

    public async Task<List<CEAAlarm>> GetAllAsync()
    {
        return await _context.CEAAlarms.ToListAsync();
    }

    public async Task<List<CEAAlarm>> GetByDeviceIdAsync(int deviceId)
    {
        return await _context.CEAAlarms
            .Where(a => a.DeviceId == deviceId)
            .ToListAsync();
    }

    public async Task<CEAAlarm> AddAsync(CEAAlarm alarm)
    {
        await _context.CEAAlarms.AddAsync(alarm);
        await _context.SaveChangesAsync();
        return alarm;
    }

    public async Task<CEAAlarm> UpdateAsync(CEAAlarm alarm)
    {
        _context.CEAAlarms.Update(alarm);
        await _context.SaveChangesAsync();
        return alarm;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var alarm = await _context.CEAAlarms.FindAsync(id);
        if (alarm == null) return false;

        _context.CEAAlarms.Remove(alarm);
        await _context.SaveChangesAsync();
        return true;
    }
}

