using alarm_service.Entities;

namespace alarm_service.Repositories;

public interface ISLBNAlarmRepository
{
    Task<SLBNAlarm?> GetByIdAsync(int id);
    Task<List<SLBNAlarm>> GetAllAsync();
    Task<List<SLBNAlarm>> GetByDeviceIdAsync(int deviceId);
    Task<SLBNAlarm> AddAsync(SLBNAlarm alarm);
    Task<SLBNAlarm> UpdateAsync(SLBNAlarm alarm);
    Task<bool> DeleteAsync(int id);
}

