using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.CoverType
{
    public class UpdateCoverTypeDTO
    {
        [Required]
        public int CoverTypeId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
    }
}
