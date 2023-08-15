using BookCommerce_WEB.Models;

namespace BookCommerce_WEB.Services.APIMessageRequestBuilder
{
    public interface IAPIMessageRequestBuilder
    {
        HttpRequestMessage Build(APIRequest request);
    }
}
