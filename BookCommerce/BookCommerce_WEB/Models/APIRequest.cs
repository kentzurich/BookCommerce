using static BookCommerce_Utility.StaticDetails;

namespace BookCommerce_WEB.Models
{
    public class APIRequest
    {
        public APIType APIType { get; set; } = APIType.GET;
        public string URL { get; set; }
        public object Data { get; set; }
        public string Token { get; set; }
        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
