using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models.DTO.Category
{
    public class UpdateCategoryDTO
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
