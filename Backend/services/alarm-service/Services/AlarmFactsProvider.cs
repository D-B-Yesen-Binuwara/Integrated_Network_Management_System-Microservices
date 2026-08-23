using alarm_service.Correlation.Models;
using alarm_service.Data;
using alarm_service.Services.Implement;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Services;

public class AlarmFactsProvider : IAlarmFactsProvider
{
    private readonly AlarmDbContext _context;

    public AlarmFactsProvider(AlarmDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<AlarmFact>> GetActiveAlarmsAsync(
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var slbn = await _context.SLBNAlarms
            .AsNoTracking()
            .Where(a => a.IsActive && a.RaisedTime >= from && a.RaisedTime <= to)
            .Select(a => new AlarmFact
            {
                AlarmId = a.SLBNAlarmId,
                DeviceId = a.DeviceId,
                DeviceType = "SLBN",
                AlarmType = a.AlarmType,
                RaisedTime = a.RaisedTime,
                IsActive = a.IsActive
            })
            .ToListAsync(cancellationToken);

        var cean = await _context.CEAAlarms
            .AsNoTracking()
            .Where(a => a.IsActive && a.RaisedTime >= from && a.RaisedTime <= to)
            .Select(a => new AlarmFact
            {
                AlarmId = a.CEAAlarmId,
                DeviceId = a.DeviceId,
                DeviceType = "CEAN",
                AlarmType = a.AlarmType,
                RaisedTime = a.RaisedTime,
                IsActive = a.IsActive
            })
            .ToListAsync(cancellationToken);

        var msan = await _context.MSANAlarms
            .AsNoTracking()
            .Where(a => a.IsActive && a.RaisedTime >= from && a.RaisedTime <= to)
            .Select(a => new AlarmFact
            {
                AlarmId = a.MSANAlarmId,
                DeviceId = a.DeviceId,
                DeviceType = "MSAN",
                AlarmType = a.AlarmType,
                RaisedTime = a.RaisedTime,
                IsActive = a.IsActive
            })
            .ToListAsync(cancellationToken);

        return slbn.Concat(cean).Concat(msan).ToList();
    }
}
