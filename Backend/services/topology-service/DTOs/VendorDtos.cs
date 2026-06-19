using System.ComponentModel.DataAnnotations;
using topology_service.Enums;

namespace topology_service.DTOs;

public record CreateVendorDto(
    [Required]
    [MaxLength(100)]
    string Name,
    
    [Required]
    [MaxLength(50)]
    string Brand,
    
    [Required]
    DeviceType DeviceType,
    
    [MaxLength(255)]
    string? Description
);

public record UpdateVendorDto(
    [Required]
    [MaxLength(100)]
    string Name,
    
    [Required]
    [MaxLength(50)]
    string Brand,
    
    [Required]
    DeviceType DeviceType,
    
    [MaxLength(255)]
    string? Description,
    
    bool IsActive
);

public record VendorDto(
    int VendorId,
    string Name,
    string Brand,
    DeviceType DeviceType,
    string? Description,
    bool IsActive,
    DateTime CreatedAt
);

// Vendor Statistics DTOs
public record VendorStatsDto(
    int VendorId,
    string Name,
    string Brand,
    int ActiveDeviceCount,
    int TotalDeviceCount,
    DateTime? LastAssignmentDate
);

public record VendorDeviceStatsDto(
    int VendorId,
    string Name,
    int ActiveDeviceCount,
    IEnumerable<DeviceAssignmentSummaryDto> RecentAssignments
);

public record DeviceAssignmentSummaryDto(
    int DeviceId,
    string DeviceName,
    DateTime AssignedDate,
    string? AssignedByUser
);