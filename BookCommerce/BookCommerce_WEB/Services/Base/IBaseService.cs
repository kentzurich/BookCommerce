using BookCommerce_WEB.Models;

namespace BookCommerce_WEB.Services.Base
{
    public interface IBaseService
    {
        APIResponse response { get; set; }
        Task<T> SendAsync<T>(APIRequest request, bool withBearer = true);
    }
}
