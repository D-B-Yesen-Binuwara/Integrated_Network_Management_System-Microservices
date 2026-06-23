using alarm_service.Entities;

namespace alarm_service.Repositories;

public interface IMSANAlarmRepository
{
    Task<MSANAlarm?> GetByIdAsync(int id);
    Task<List<MSANAlarm>> GetAllAsync();
    Task<List<MSANAlarm>> GetByDeviceIdAsync(int deviceId);
    Task<MSANAlarm> AddAsync(MSANAlarm alarm);
    Task<MSANAlarm> UpdateAsync(MSANAlarm alarm);
    Task<bool> DeleteAsync(int id);
}

