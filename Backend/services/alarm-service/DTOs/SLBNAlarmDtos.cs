using System.ComponentModel.DataAnnotations;

namespace alarm_service.DTOs;

public record SLBNAlarmQueryParams(
    bool? IsActive = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? DeviceId = null,
    string? SortBy = null,
    string? Order = "desc"
);

public record SLBNAlarmListDto(
    int SLBNAlarmId,
    int DeviceId,
    string AlarmType,
    DateTime RaisedTime,
    DateTime? ClearedTime,
    bool IsActive
);

public class CreateSLBNAlarmRequestDto
{
    [Required]
    public int DeviceId { get; set; }

    [Required]
    public string AlarmType { get; set; } = string.Empty;

    public DateTime? ClearedTime { get; set; }
}

public class UpdateSLBNAlarmRequestDto
{
    [Required]
    public int DeviceId { get; set; }

    [Required]
    public string AlarmType { get; set; } = string.Empty;

    public DateTime? ClearedTime { get; set; }

    // Monolith does not validate IsActive in controller; keep it updatable.
    public bool IsActive { get; set; }
}

public record SLBNAlarmResponseDto(
    int SLBNAlarmId,
    int DeviceId,
    string AlarmType,
    DateTime RaisedTime,
    DateTime? ClearedTime,
    bool IsActive
);

