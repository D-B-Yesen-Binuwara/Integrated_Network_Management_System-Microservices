using alarm_service.Data;
using alarm_service.Correlation.Engine;
using alarm_service.Correlation.Models;
using alarm_service.DTOs;
using alarm_service.Entities;
using alarm_service.Repositories;
using alarm_service.Repositories.Interfaces;
using alarm_service.Services.Implement;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Services;

public class MSANAlarmService : IMSANAlarmService
{
    private readonly IMSANAlarmRepository _repository;
    private readonly AlarmDbContext _context;
    private readonly IImpactAnalysisService _impactAnalysisService;
    private readonly ICorrelationEngine _correlationEngine;

    public MSANAlarmService(IMSANAlarmRepository repository, AlarmDbContext context, IImpactAnalysisService impactAnalysisService, ICorrelationEngine correlationEngine)
    {
        _repository = repository;
        _context = context;
        _impactAnalysisService = impactAnalysisService;
        _correlationEngine = correlationEngine;
    }

    public async Task<MSANAlarmResponseDto?> GetByIdAsync(int id)
    {
        var alarm = await _repository.GetByIdAsync(id);
        return alarm == null ? null : ToResponseDto(alarm);
    }

    public async Task<List<MSANAlarmResponseDto>> GetAllAsync()
    {
        var alarms = await _repository.GetAllAsync();
        return alarms.Select(ToResponseDto).ToList();
    }

    public async Task<List<MSANAlarmResponseDto>> GetByDeviceIdAsync(int deviceId)
    {
        var alarms = await _repository.GetByDeviceIdAsync(deviceId);
        return alarms.Select(ToResponseDto).ToList();
    }

    public async Task<MSANAlarmResponseDto> CreateAsync(CreateMSANAlarmRequestDto dto)
    {
        var alarm = new MSANAlarm
        {
            DeviceId = dto.DeviceId,
            RegionCode = dto.RegionCode,
            ProvinceCode = dto.ProvinceCode,
            LEACode = dto.LEACode,
            AlarmType = dto.AlarmType,
            ClearedTime = dto.ClearedTime,
            RaisedTime = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repository.AddAsync(alarm);

        await _correlationEngine.EvaluateAsync(new CorrelationContext
        {
            AlarmId = created.MSANAlarmId,
            DeviceId = created.DeviceId,
            AlarmType = created.AlarmType,
            DeviceType = "MSAN",
            RaisedTime = created.RaisedTime
        });

        return ToResponseDto(created);
    }

    public async Task<MSANAlarmResponseDto?> UpdateAsync(int id, UpdateMSANAlarmRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        var updatedAlarm = new MSANAlarm
        {
            MSANAlarmId = id,
            DeviceId = dto.DeviceId,
            RegionCode = dto.RegionCode,
            ProvinceCode = dto.ProvinceCode,
            LEACode = dto.LEACode,
            AlarmType = dto.AlarmType,
            RaisedTime = existing.RaisedTime,
            ClearedTime = dto.ClearedTime,
            IsActive = dto.IsActive
        };

        var updated = await _repository.UpdateAsync(updatedAlarm);

        if (existing.IsActive && !updated.IsActive)
        {
            await _impactAnalysisService.ClearRootCauseAsync(updated.DeviceId);
        }
        else if (updated.IsActive)
        {
            await _correlationEngine.EvaluateAsync(new CorrelationContext
            {
                AlarmId = updated.MSANAlarmId,
                DeviceId = updated.DeviceId,
                AlarmType = updated.AlarmType,
                DeviceType = "MSAN",
                RaisedTime = updated.RaisedTime
            });
        }

        return ToResponseDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<List<MSANAlarmListDto>> GetFilteredAsync(MSANAlarmQueryParams queryParams)
    {
        var query = _context.MSANAlarms.AsNoTracking().AsQueryable();

        if (queryParams.IsActive.HasValue)
            query = query.Where(a => a.IsActive == queryParams.IsActive.Value);

        if (queryParams.DateFrom.HasValue)
            query = query.Where(a => a.RaisedTime >= queryParams.DateFrom.Value);

        if (queryParams.DateTo.HasValue)
            query = query.Where(a => a.RaisedTime <= queryParams.DateTo.Value);

        if (queryParams.DeviceId.HasValue)
            query = query.Where(a => a.DeviceId == queryParams.DeviceId.Value);

        var order = queryParams.Order?.ToLower() ?? "desc";
        var sortBy = queryParams.SortBy?.ToLower() ?? "raisedtime";

        query = sortBy switch
        {
            "alarmtype" => order == "desc"
                ? query.OrderByDescending(a => a.AlarmType)
                : query.OrderBy(a => a.AlarmType),
            _ => order == "desc"
                ? query.OrderByDescending(a => a.RaisedTime)
                : query.OrderBy(a => a.RaisedTime)
        };

        return await query
            .Select(a => new MSANAlarmListDto(
                a.MSANAlarmId,
                a.DeviceId,
                a.RegionCode,
                a.ProvinceCode,
                a.LEACode,
                a.AlarmType,
                a.RaisedTime,
                a.ClearedTime,
                a.IsActive
            ))
            .ToListAsync();
    }

    public async Task<List<MSANAlarmResponseDto>> GetActiveAsync()
    {
        var all = await _repository.GetAllAsync();
        return all.Where(a => a.IsActive).Select(ToResponseDto).ToList();
    }

    private static MSANAlarmResponseDto ToResponseDto(MSANAlarm alarm) =>
        new(
            alarm.MSANAlarmId,
            alarm.DeviceId,
            alarm.RegionCode,
            alarm.ProvinceCode,
            alarm.LEACode,
            alarm.AlarmType,
            alarm.RaisedTime,
            alarm.ClearedTime,
            alarm.IsActive
        );
}

