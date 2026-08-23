using System.ComponentModel.DataAnnotations;

namespace alarm_service.Entities;

public class MSANAlarm
{
    [Key]
    public int MSANAlarmId { get; set; }

    public int DeviceId { get; set; }

    public string RegionCode { get; set; } = string.Empty;
    public string ProvinceCode { get; set; } = string.Empty;
    public string LEACode { get; set; } = string.Empty;

    public string AlarmType { get; set; } = string.Empty;

    public DateTime RaisedTime { get; set; }

    public DateTime? ClearedTime { get; set; }

    public bool IsActive { get; set; }
}

