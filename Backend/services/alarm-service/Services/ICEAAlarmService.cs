using alarm_service.DTOs;

namespace alarm_service.Services;

public interface ICEAAlarmService
{
    Task<CEAAlarmResponseDto?> GetByIdAsync(int id);
    Task<List<CEAAlarmResponseDto>> GetAllAsync();
    Task<List<CEAAlarmResponseDto>> GetActiveAsync();
    Task<List<CEAAlarmResponseDto>> GetByDeviceIdAsync(int deviceId);

    Task<CEAAlarmResponseDto> CreateAsync(CreateCEAAlarmRequestDto dto);
    Task<CEAAlarmResponseDto?> UpdateAsync(int id, UpdateCEAAlarmRequestDto dto);
    Task<bool> DeleteAsync(int id);

    Task<List<CEAAlarmListDto>> GetFilteredAsync(CEAAlarmQueryParams queryParams);
}

