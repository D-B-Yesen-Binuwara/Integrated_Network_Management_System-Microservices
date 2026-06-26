using alarm_service.Data;
using alarm_service.DTOs;
using alarm_service.Entities;
using alarm_service.Repositories;
using alarm_service.Repositories.Interfaces;
using alarm_service.Services.Implement;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Services;

public class SLBNAlarmService : ISLBNAlarmService
{
    private readonly ISLBNAlarmRepository _repository;
    private readonly AlarmDbContext _context;
    private readonly IImpactAnalysisService _impactAnalysisService;

    public SLBNAlarmService(ISLBNAlarmRepository repository, AlarmDbContext context, IImpactAnalysisService impactAnalysisService)
    {
        _repository = repository;
        _context = context;
        _impactAnalysisService = impactAnalysisService;
    }

    public async Task<SLBNAlarmResponseDto?> GetByIdAsync(int id)
    {
        var alarm = await _repository.GetByIdAsync(id);
        return alarm == null ? null : ToResponseDto(alarm);
    }

    public async Task<List<SLBNAlarmResponseDto>> GetAllAsync()
    {
        var alarms = await _repository.GetAllAsync();
        return alarms.Select(ToResponseDto).ToList();
    }

    public async Task<List<SLBNAlarmResponseDto>> GetByDeviceIdAsync(int deviceId)
    {
        var alarms = await _repository.GetByDeviceIdAsync(deviceId);
        return alarms.Select(ToResponseDto).ToList();
    }

    public async Task<SLBNAlarmResponseDto> CreateAsync(CreateSLBNAlarmRequestDto dto)
    {
        var alarm = new SLBNAlarm
        {
            DeviceId = dto.DeviceId,
            AlarmType = dto.AlarmType,
            ClearedTime = dto.ClearedTime,
            RaisedTime = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repository.AddAsync(alarm);

        if (created.AlarmType == "NODE_DOWN")
        {
            await _impactAnalysisService.AnalyzeFailureAsync(created.DeviceId, created.SLBNAlarmId);
        }

        return ToResponseDto(created);
    }

    public async Task<SLBNAlarmResponseDto?> UpdateAsync(int id, UpdateSLBNAlarmRequestDto dto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return null;

        var updatedAlarm = new SLBNAlarm
        {
            SLBNAlarmId = id,
            DeviceId = dto.DeviceId,
            AlarmType = dto.AlarmType,
            RaisedTime = existing.RaisedTime,
            ClearedTime = dto.ClearedTime,
            IsActive = dto.IsActive
        };

        var updated = await _repository.UpdateAsync(updatedAlarm);

        if (existing.IsActive && !updated.IsActive && updated.AlarmType == "NODE_DOWN")
        {
            await _impactAnalysisService.ClearRootCauseAsync(updated.DeviceId);
        }

        return ToResponseDto(updated);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    public async Task<List<SLBNAlarmListDto>> GetFilteredAsync(SLBNAlarmQueryParams queryParams)
    {
        var query = _context.SLBNAlarms.AsNoTracking().AsQueryable();

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
            .Select(a => new SLBNAlarmListDto(
                a.SLBNAlarmId,
                a.DeviceId,
                a.AlarmType,
                a.RaisedTime,
                a.ClearedTime,
                a.IsActive
            ))
            .ToListAsync();
    }

    public async Task<List<SLBNAlarmResponseDto>> GetActiveAsync()
    {
        var all = await _repository.GetAllAsync();
        return all.Where(a => a.IsActive).Select(ToResponseDto).ToList();
    }

    private static SLBNAlarmResponseDto ToResponseDto(SLBNAlarm alarm) =>
        new(
            alarm.SLBNAlarmId,
            alarm.DeviceId,
            alarm.AlarmType,
            alarm.RaisedTime,
            alarm.ClearedTime,
            alarm.IsActive
        );
}

