using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alarm_service.Entities;

public class ImpactedDevice
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ImpactedDeviceId { get; set; }

    [Required]
    public int RootCauseId { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [Required]
    [MaxLength(100)]
    public string ImpactType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string DeviceType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(RootCauseId))]
    public RootCause RootCause { get; set; } = null!;
}
