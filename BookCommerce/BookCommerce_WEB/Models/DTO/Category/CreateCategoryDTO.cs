using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models.DTO.Category
{
    public class CreateCategoryDTO
    {
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
