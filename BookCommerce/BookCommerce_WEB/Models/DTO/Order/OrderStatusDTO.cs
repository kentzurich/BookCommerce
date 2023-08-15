namespace BookCommerce_WEB.Models.DTO.Order
{
	public class OrderStatusDTO
	{
        public int OrderHeaderId { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentStatus { get; set; }
    }
}
