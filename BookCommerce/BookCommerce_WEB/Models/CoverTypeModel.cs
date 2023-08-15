using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models
{
    public class CoverTypeModel
    {
        [Key]
        public int CoverTypeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
