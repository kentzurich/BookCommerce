using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models
{
    public class RefreshToken
    {
        [Key]
        public int RefreshTokenId { get; set; }
        public string UserId { get; set; }
        public string JwtTokenId { get; set; }
        public string Refresh_Token { get; set; }
        //We will make sure that the refresh token is valid at one use 
        public bool IsValid { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
