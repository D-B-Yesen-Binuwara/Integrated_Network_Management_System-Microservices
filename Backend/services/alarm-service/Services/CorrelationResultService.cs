using alarm_service.Correlation.Models;
using alarm_service.Data;
using alarm_service.Entities;
using alarm_service.Services.Implement;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Services;

public class CorrelationResultService : ICorrelationResultService
{
    private readonly AlarmDbContext _context;
    private readonly ILogger<CorrelationResultService> _logger;

    public CorrelationResultService(AlarmDbContext context, ILogger<CorrelationResultService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task PersistAsync(CorrelationResult result, CancellationToken cancellationToken = default)
    {
        if (!result.RootCauseDeviceId.HasValue)
        {
            return;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var rootCause = await _context.RootCauses
            .FirstOrDefaultAsync(
                root => root.DeviceId == result.RootCauseDeviceId.Value &&
                        root.AlarmId == result.SourceAlarmId &&
                        root.SourceDeviceType == result.SourceDeviceType,
                cancellationToken);

        if (rootCause is null)
        {
            rootCause = new RootCause
            {
                DeviceId = result.RootCauseDeviceId.Value,
                AlarmId = result.SourceAlarmId,
                RootCauseType = result.SourceAlarmType,
                SourceDeviceType = result.SourceDeviceType,
                CorrelationRuleName = result.MatchedRuleName ?? string.Empty,
                CreatedAt = result.CorrelationTime
            };
            _context.RootCauses.Add(rootCause);
        }
        else
        {
            rootCause.RootCauseType = result.SourceAlarmType;
            rootCause.SourceDeviceType = result.SourceDeviceType;
            rootCause.CorrelationRuleName = result.MatchedRuleName ?? rootCause.CorrelationRuleName;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var existingImpacts = await _context.ImpactedDevices
            .Where(impact => impact.RootCauseId == rootCause.RootCauseId)
            .ToListAsync(cancellationToken);
        _context.ImpactedDevices.RemoveRange(existingImpacts);

        var impactedDevices = result.ImpactedDevices
            .Where(deviceId => deviceId != rootCause.DeviceId)
            .Distinct()
            .Select(deviceId => new ImpactedDevice
            {
                RootCauseId = rootCause.RootCauseId,
                DeviceId = deviceId,
                ImpactType = "DOWNSTREAM",
                DeviceType = result.TargetDeviceType ?? string.Empty,
                CreatedAt = result.CorrelationTime
            });
        await _context.ImpactedDevices.AddRangeAsync(impactedDevices, cancellationToken);

        var existingFault = await _context.CorrelatedFaults
            .Include(fault => fault.SuppressedAlarms)
            .FirstOrDefaultAsync(
                fault => fault.SourceDeviceType == result.SourceDeviceType &&
                         fault.SourceAlarmId == result.SourceAlarmId,
                cancellationToken);

        if (existingFault is null)
        {
            existingFault = new CorrelatedFault
            {
                RootCauseId = rootCause.RootCauseId,
                CorrelationRuleName = result.MatchedRuleName ?? string.Empty,
                SourceDeviceId = result.SourceDeviceId,
                SourceDeviceType = result.SourceDeviceType,
                SourceAlarmId = result.SourceAlarmId,
                SourceAlarmType = result.SourceAlarmType,
                StartedAt = result.CorrelationTime,
                Status = "ACTIVE",
                ConfidenceScore = CalculateConfidence(result)
            };
            _context.CorrelatedFaults.Add(existingFault);
        }
        else
        {
            existingFault.RootCauseId = rootCause.RootCauseId;
            existingFault.CorrelationRuleName = result.MatchedRuleName ?? existingFault.CorrelationRuleName;
            existingFault.ConfidenceScore = CalculateConfidence(result);
            _context.CorrelatedFaultAlarms.RemoveRange(existingFault.SuppressedAlarms);
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (result.SuppressedAlarmReferences.Count > 0)
        {
            var suppressed = result.SuppressedAlarmReferences.Select(alarm => new CorrelatedFaultAlarm
            {
                CorrelatedFaultId = existingFault.CorrelatedFaultId,
                AlarmId = alarm.AlarmId,
                DeviceId = alarm.DeviceId,
                DeviceType = alarm.DeviceType,
                AlarmType = alarm.AlarmType,
                RaisedTime = alarm.RaisedTime
            });
            await _context.CorrelatedFaultAlarms.AddRangeAsync(suppressed, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        result.CorrelatedFaultId = existingFault.CorrelatedFaultId;

        _logger.LogInformation(
            "Persisted correlation fault {FaultId} for {DeviceType} alarm {AlarmId}",
            existingFault.CorrelatedFaultId,
            result.SourceDeviceType,
            result.SourceAlarmId);
    }

    public async Task ClearForDeviceAsync(int deviceId, CancellationToken cancellationToken = default)
    {
        var roots = await _context.RootCauses
            .Where(root => root.DeviceId == deviceId)
            .ToListAsync(cancellationToken);
        if (roots.Count == 0) return;

        var rootIds = roots.Select(root => root.RootCauseId).ToList();
        var faults = await _context.CorrelatedFaults
            .Where(fault => rootIds.Contains(fault.RootCauseId))
            .ToListAsync(cancellationToken);
        foreach (var fault in faults)
        {
            fault.Status = "CLEARED";
            fault.EndedAt = DateTime.UtcNow;
        }

        _context.RootCauses.RemoveRange(roots);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private static decimal CalculateConfidence(CorrelationResult result)
    {
        var score = result.RootCauseDeviceId.HasValue ? 0.5m : 0m;
        if (result.ImpactedDevices.Count > 0) score += 0.25m;
        if (result.SuppressedAlarmReferences.Count > 0) score += 0.25m;
        return score;
    }
}
