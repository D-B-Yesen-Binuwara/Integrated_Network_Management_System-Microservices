using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alarm_service.Entities;

public class RootCause
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RootCauseId { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [Required]
    public int AlarmId { get; set; }

    [Required]
    [MaxLength(100)]
    public string RootCauseType { get; set; } = string.Empty;

    [MaxLength(50)]
    public string SourceDeviceType { get; set; } = string.Empty;

    [MaxLength(150)]
    public string CorrelationRuleName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ImpactedDevice> ImpactedDevices { get; set; } = new List<ImpactedDevice>();
}
