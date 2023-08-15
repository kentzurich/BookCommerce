using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.Category
{
    public class CreateCategoryDTO
    {
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
