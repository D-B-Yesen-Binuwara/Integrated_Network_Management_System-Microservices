using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alarm_service.Entities;

public class CorrelatedFault
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CorrelatedFaultId { get; set; }

    [Required]
    public int RootCauseId { get; set; }

    [Required]
    [MaxLength(150)]
    public string CorrelationRuleName { get; set; } = string.Empty;

    [Required]
    public int SourceDeviceId { get; set; }

    [Required]
    [MaxLength(50)]
    public string SourceDeviceType { get; set; } = string.Empty;

    [Required]
    public int SourceAlarmId { get; set; }

    [Required]
    [MaxLength(100)]
    public string SourceAlarmType { get; set; } = string.Empty;

    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAt { get; set; }

    [Required]
    [MaxLength(30)]
    public string Status { get; set; } = "ACTIVE";

    public decimal ConfidenceScore { get; set; }

    [ForeignKey(nameof(RootCauseId))]
    public RootCause RootCause { get; set; } = null!;

    public ICollection<CorrelatedFaultAlarm> SuppressedAlarms { get; set; } = new List<CorrelatedFaultAlarm>();
}

public class CorrelatedFaultAlarm
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int CorrelatedFaultAlarmId { get; set; }

    [Required]
    public int CorrelatedFaultId { get; set; }

    [Required]
    public int AlarmId { get; set; }

    [Required]
    public int DeviceId { get; set; }

    [Required]
    [MaxLength(50)]
    public string DeviceType { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string AlarmType { get; set; } = string.Empty;

    public DateTime RaisedTime { get; set; }

    [ForeignKey(nameof(CorrelatedFaultId))]
    public CorrelatedFault CorrelatedFault { get; set; } = null!;
}
