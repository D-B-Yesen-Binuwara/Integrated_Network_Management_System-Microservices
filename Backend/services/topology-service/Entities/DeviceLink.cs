using System.ComponentModel.DataAnnotations;

namespace topology_service.Entities;

public class DeviceLink
{
    [Key]
    public int LinkId { get; set; }

    [Required]
    public int ParentDeviceId { get; set; }
    public Device? ParentDevice { get; set; }

    [Required]
    public int ChildDeviceId { get; set; }
    public Device? ChildDevice { get; set; }

    [Required]
    [MaxLength(50)]
    public string LinkStatus { get; set; } = "UP";
}
