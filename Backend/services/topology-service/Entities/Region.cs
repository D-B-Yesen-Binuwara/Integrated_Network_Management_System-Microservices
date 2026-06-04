using System.ComponentModel.DataAnnotations;

namespace topology_service.Entities
{
    public class Region
    {
        [Key]
        public int RegionId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}
