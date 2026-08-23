using System.ComponentModel.DataAnnotations;

namespace INMS.Identity.Domain.Entities;

public class UserAreaAssignment
{
    [Key]
    public int AssignmentId { get; set; }

    public int UserId { get; set; }

    public string AreaType { get; set; } = string.Empty;

    public Guid AreaId { get; set; }

    // Human-readable topology codes keep identity assignments aligned with
    // the separate topology database without introducing cross-database FKs.
    public string? RegionCode { get; set; }
    public string? ProvinceCode { get; set; }
    public string? LEACode { get; set; }

    public User? User { get; set; }
}
