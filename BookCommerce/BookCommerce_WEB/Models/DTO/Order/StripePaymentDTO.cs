namespace BookCommerce_WEB.Models.DTO.Order
{
	public class StripePaymentDTO
	{
        public int OrderHeaderId { get; set; }
        public string SessionId { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
