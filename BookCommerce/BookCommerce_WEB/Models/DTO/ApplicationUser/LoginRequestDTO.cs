using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models.DTO.ApplicationUser
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
