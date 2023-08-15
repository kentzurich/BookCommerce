using BookCommerce_WEB.Models.DTO.ApplicationUser;

namespace BookCommerce_WEB.Services.TokenProvider
{
    public interface ITokenProvider
    {
        void SetToken(TokenDTO tokenDTO);
        TokenDTO? GetToken();
        void ClearToken();
    }
}
