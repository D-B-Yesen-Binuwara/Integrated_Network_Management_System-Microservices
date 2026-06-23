using alarm_service.DTOs;

namespace alarm_service.Services;

public interface ISLBNAlarmService
{
    Task<SLBNAlarmResponseDto?> GetByIdAsync(int id);
    Task<List<SLBNAlarmResponseDto>> GetAllAsync();
    Task<List<SLBNAlarmResponseDto>> GetActiveAsync();
    Task<List<SLBNAlarmResponseDto>> GetByDeviceIdAsync(int deviceId);

    Task<SLBNAlarmResponseDto> CreateAsync(CreateSLBNAlarmRequestDto dto);
    Task<SLBNAlarmResponseDto?> UpdateAsync(int id, UpdateSLBNAlarmRequestDto dto);
    Task<bool> DeleteAsync(int id);

    Task<List<SLBNAlarmListDto>> GetFilteredAsync(SLBNAlarmQueryParams queryParams);
}

