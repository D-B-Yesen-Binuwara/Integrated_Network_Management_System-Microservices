using System.ComponentModel.DataAnnotations;

namespace topology_service.Entities
{
    public class LEA
    {
        [Key]
        public int LEAId { get; set; }
        
        [Required]
        [MaxLength(20)]
        public string LEACode { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;
        
        public int ProvinceId { get; set; }
        public Province? Province { get; set; }
    }
}
