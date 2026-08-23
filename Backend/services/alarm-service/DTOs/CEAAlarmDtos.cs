using System.ComponentModel.DataAnnotations;

namespace alarm_service.DTOs;

public record CEAAlarmQueryParams(
    bool? IsActive = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    int? DeviceId = null,
    string? SortBy = null,
    string? Order = "desc"
);

public record CEAAlarmListDto(
    int CEAAlarmId,
    int DeviceId,
    string RegionCode,
    string ProvinceCode,
    string LEACode,
    string AlarmType,
    DateTime RaisedTime,
    DateTime? ClearedTime,
    bool IsActive
);

public class CreateCEAAlarmRequestDto
{
    [Required]
    public int DeviceId { get; set; }

    public string RegionCode { get; set; } = string.Empty;
    public string ProvinceCode { get; set; } = string.Empty;
    public string LEACode { get; set; } = string.Empty;

    [Required]
    public string AlarmType { get; set; } = string.Empty;

    public DateTime? ClearedTime { get; set; }
}

public class UpdateCEAAlarmRequestDto
{
    [Required]
    public int DeviceId { get; set; }

    public string RegionCode { get; set; } = string.Empty;
    public string ProvinceCode { get; set; } = string.Empty;
    public string LEACode { get; set; } = string.Empty;

    [Required]
    public string AlarmType { get; set; } = string.Empty;

    public DateTime? ClearedTime { get; set; }

    // Monolith does not validate IsActive in controller; keep it updatable.
    public bool IsActive { get; set; }
}

public record CEAAlarmResponseDto(
    int CEAAlarmId,
    int DeviceId,
    string RegionCode,
    string ProvinceCode,
    string LEACode,
    string AlarmType,
    DateTime RaisedTime,
    DateTime? ClearedTime,
    bool IsActive
);

