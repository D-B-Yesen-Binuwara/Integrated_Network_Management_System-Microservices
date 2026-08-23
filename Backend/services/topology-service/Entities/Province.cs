using System.ComponentModel.DataAnnotations;

namespace topology_service.Entities
{
    public class Province
    {
        [Key]
        public int ProvinceId { get; set; }

        [Required]
        [MaxLength(20)]
        public string ProvinceCode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        public int RegionId { get; set; }

        public Region? Region { get; set; }
    }
}
