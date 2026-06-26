using alarm_service.DTOs;

namespace alarm_service.Services.Implement;

public interface IMSANAlarmService
{
    Task<MSANAlarmResponseDto?> GetByIdAsync(int id);
    Task<List<MSANAlarmResponseDto>> GetAllAsync();
    Task<List<MSANAlarmResponseDto>> GetActiveAsync();
    Task<List<MSANAlarmResponseDto>> GetByDeviceIdAsync(int deviceId);

    Task<MSANAlarmResponseDto> CreateAsync(CreateMSANAlarmRequestDto dto);
    Task<MSANAlarmResponseDto?> UpdateAsync(int id, UpdateMSANAlarmRequestDto dto);
    Task<bool> DeleteAsync(int id);

    Task<List<MSANAlarmListDto>> GetFilteredAsync(MSANAlarmQueryParams queryParams);
}

