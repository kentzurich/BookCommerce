using BookCommerce_WEB.Models.DTO.Order;
using BookCommerce_WEB.Models.DTO.Order.Details;
using BookCommerce_WEB.Models.DTO.Order.Header;

namespace BookCommerce_WEB.Services.Order
{
	public interface IOrderService
	{
        Task<T> GetOrderHeaderAsync<T>(string applicationUserId = "all", int orderHeaderId = 0);
		Task<T> GetOrderDetailsAsync<T>(int orderHeaderId);
        Task<T> UpdateStatusAsync<T>(OrderStatusDTO model);
		Task<T> CreateOrderHeaderAsync<T>(CreateOrderHeaderDTO model);
		Task<T> CreateOrderDetailsAsync<T>(CreateOrderDetailsDTO model);
		Task<T> UpdateStripePaymentAsync<T>(StripePaymentDTO model);
		Task<T> UpdateOrderHeaderAsync<T>(UpdateOrderHeaderDTO model);
	}
}
