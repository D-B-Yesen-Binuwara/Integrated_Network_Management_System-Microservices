using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using topology_service.Entities;

namespace topology_service.Entities
{
    public class DeviceVendor
    {
        [Key]
        public int DeviceVendorId { get; set; }

        [Required]
        [ForeignKey(nameof(Device))]
        public int DeviceId { get; set; }

        [Required]
        [ForeignKey(nameof(Vendor))]
        public int VendorId { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }

        [MaxLength(100)]
        public string? AssignedByUser { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Device? Device { get; set; }
        public virtual Vendor? Vendor { get; set; }
    }
}
