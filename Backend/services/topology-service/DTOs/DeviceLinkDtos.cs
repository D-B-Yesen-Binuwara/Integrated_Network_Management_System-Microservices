using System.ComponentModel.DataAnnotations;

namespace topology_service.DTOs;

public class DeviceLinkDto
{
    public int LinkId { get; set; }
    public int ParentDeviceId { get; set; }
    public int ChildDeviceId { get; set; }
    public string LinkStatus { get; set; } = "UP";
}

public class CreateDeviceLinkDto
{
    [Required]
    public int ParentDeviceId { get; set; }

    [Required]
    public int ChildDeviceId { get; set; }
}
