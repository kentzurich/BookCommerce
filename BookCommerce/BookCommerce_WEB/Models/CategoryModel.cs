using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [Range(1, 100, ErrorMessage = "Display Order must be between 1 and 100 only.")]
        public int DisplayOrder { get; set; }
    }
}
