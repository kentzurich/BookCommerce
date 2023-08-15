using BookCommerce_Utility;
using BookCommerce_WEB.Models.DTO.ApplicationUser;

namespace BookCommerce_WEB.Services.TokenProvider
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void ClearToken()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.AccessToken);
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(StaticDetails.RefreshToken);
        }

        public TokenDTO GetToken()
        {
            try
            {
                bool hasAccessToken = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticDetails.AccessToken, out var accessToken);
                bool hasRefreshToken = _httpContextAccessor.HttpContext.Request.Cookies.TryGetValue(StaticDetails.RefreshToken, out var refreshToken);
                TokenDTO tokenDTO = new()
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                };
                return hasAccessToken ? tokenDTO : null;
            }
            catch
            {
                return null;
            }
        }

        public void SetToken(TokenDTO tokenDTO)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.Now.AddMonths(1) };
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.AccessToken, tokenDTO.AccessToken, cookieOptions);
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(StaticDetails.RefreshToken, tokenDTO.RefreshToken, cookieOptions);
        }
    }
}
