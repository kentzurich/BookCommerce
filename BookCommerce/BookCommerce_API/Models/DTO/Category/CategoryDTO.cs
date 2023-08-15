using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.Category
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
