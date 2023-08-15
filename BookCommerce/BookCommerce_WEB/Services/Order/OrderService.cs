using BookCommerce_Utility;
using BookCommerce_WEB.Models;
using BookCommerce_WEB.Models.DTO.Order;
using BookCommerce_WEB.Models.DTO.Order.Details;
using BookCommerce_WEB.Models.DTO.Order.Header;
using BookCommerce_WEB.Services.Base;

namespace BookCommerce_WEB.Services.Order
{
    public class OrderService : IOrderService
	{
		private readonly string bookCommerceUrl;
		private readonly string APIName;
		private readonly IBaseService _baseService;

		public OrderService(IConfiguration config, IBaseService baseService)
		{
			_baseService = baseService;
			bookCommerceUrl = config.GetValue<string>("ServiceUrls:BookCommerce_API");
			APIName = "OrderAPI";
		}

		public async Task<T> CreateOrderHeaderAsync<T>(CreateOrderHeaderDTO model)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.POST,
					Data = model,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/CreateOrderHeader"
				});
		}

		public async Task<T> CreateOrderDetailsAsync<T>(CreateOrderDetailsDTO model)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.POST,
					Data = model,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/CreateOrderDetails"
				});
		}

		public async Task<T> UpdateStripePaymentAsync<T>(StripePaymentDTO model)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.PUT,
					Data = model,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/UpdateStripePayment"
				});
		}

		public async Task<T> UpdateStatusAsync<T>(OrderStatusDTO model)
		{
			return await _baseService.SendAsync<T>(
				new APIRequest()
				{
					APIType = StaticDetails.APIType.PUT,
					Data = model,
					URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/UpdateStatus"
				});
		}

        public async Task<T> GetOrderHeaderAsync<T>(string applicationUserId = "all", int orderHeaderId = 0)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/{applicationUserId}/{orderHeaderId}"
                });
        }

        public async Task<T> GetOrderDetailsAsync<T>(int orderHeaderId)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.GET,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/{orderHeaderId}"
                });
        }

        public async Task<T> UpdateOrderHeaderAsync<T>(UpdateOrderHeaderDTO model)
        {
            return await _baseService.SendAsync<T>(
                new APIRequest()
                {
                    APIType = StaticDetails.APIType.PUT,
                    Data = model,
                    URL = $"{bookCommerceUrl}/api/{StaticDetails.CurrentAPIVersion}/{APIName}/UpdateOrderHeader"
                });
        }
    }
}
