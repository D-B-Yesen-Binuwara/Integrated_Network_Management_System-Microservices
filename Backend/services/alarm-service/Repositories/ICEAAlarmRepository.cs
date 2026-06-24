using alarm_service.Entities;

namespace alarm_service.Repositories;

public interface ICEAAlarmRepository
{
    Task<CEAAlarm?> GetByIdAsync(int id);
    Task<List<CEAAlarm>> GetAllAsync();
    Task<List<CEAAlarm>> GetByDeviceIdAsync(int deviceId);
    Task<CEAAlarm> AddAsync(CEAAlarm alarm);
    Task<CEAAlarm> UpdateAsync(CEAAlarm alarm);
    Task<bool> DeleteAsync(int id);
}

