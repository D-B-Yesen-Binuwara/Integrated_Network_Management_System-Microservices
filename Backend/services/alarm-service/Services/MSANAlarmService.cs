using alarm_service.Data;
using alarm_service.DTOs;
using alarm_service.Entities;
using alarm_service.Repositories;
using Microsoft.EntityFrameworkCore;

namespace alarm_service.Services;

public class MSANAlarmService : IMSANAlarmService
{
    private readonly IMSANAlarmRepository _repository;
    private readonly AlarmDbContext _context;

    public MSANAlarmService(IMSANAlarmRepository repository, AlarmDbContext context)
    {
        _repository = repository;
        _context = context;
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
            AlarmType = dto.AlarmType,
            ClearedTime = dto.ClearedTime,
            RaisedTime = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _repository.AddAsync(alarm);
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
            AlarmType = dto.AlarmType,
            RaisedTime = existing.RaisedTime,
            ClearedTime = dto.ClearedTime,
            IsActive = dto.IsActive
        };

        var updated = await _repository.UpdateAsync(updatedAlarm);
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
            alarm.AlarmType,
            alarm.RaisedTime,
            alarm.ClearedTime,
            alarm.IsActive
        );
}

