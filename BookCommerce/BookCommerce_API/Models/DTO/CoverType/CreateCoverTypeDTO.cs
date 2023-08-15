using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.CoverType
{
    public class CreateCoverTypeDTO
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
