using System.ComponentModel.DataAnnotations;
using topology_service.Enums;

namespace topology_service.DTOs;

public class DeviceDto
{
    public int DeviceId { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public DeviceType DeviceType { get; set; }
    public string IP { get; set; } = string.Empty;
    public DeviceStatus Status { get; set; }
    public PriorityLevel PriorityLevel { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsSimulatedDown { get; set; }
}

public class CreateDeviceDto
{
    [Required]
    [MaxLength(100)]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public DeviceType DeviceType { get; set; }

    [MaxLength(50)]
    public string? IP { get; set; }

    [Required]
    public PriorityLevel PriorityLevel { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal Longitude { get; set; }
}

public class UpdateDeviceDto
{
    [Required]
    [MaxLength(100)]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public DeviceType DeviceType { get; set; }

    [MaxLength(50)]
    public string? IP { get; set; }

    [Required]
    public DeviceStatus Status { get; set; }

    [Required]
    public PriorityLevel PriorityLevel { get; set; }

    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal Latitude { get; set; }

    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal Longitude { get; set; }
}
