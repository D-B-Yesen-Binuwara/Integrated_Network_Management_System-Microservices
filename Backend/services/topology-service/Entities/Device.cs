using System.ComponentModel.DataAnnotations;
using topology_service.Enums;

namespace topology_service.Entities;

public class Device
{
    [Key]
    public int DeviceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string DeviceName { get; set; } = string.Empty;

    [Required]
    public DeviceType DeviceType { get; set; }

    [Required]
    [MaxLength(50)]
    public string IP { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string RegionCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string ProvinceCode { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string LEACode { get; set; } = string.Empty;

    // Employee IDs belong to the separate identity database, so this is an
    // application-level reference rather than a cross-database foreign key.
    public int AssignedEngineerId { get; set; }

    [Required]
    public DeviceStatus Status { get; set; } = DeviceStatus.UP;

    [Required]
    public PriorityLevel PriorityLevel { get; set; } = PriorityLevel.Low;

    [Required]
    [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
    public decimal Latitude { get; set; }

    [Required]
    [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
    public decimal Longitude { get; set; }
}
