using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models
{
    public class CategoryModel
    {
        [Key]
        public int CategoryId { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
