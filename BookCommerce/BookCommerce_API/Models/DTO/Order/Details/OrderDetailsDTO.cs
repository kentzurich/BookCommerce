namespace BookCommerce_API.Models.DTO.Order.Details
{
    public class OrderDetailsDTO
    {
        public int OrderDetailsId { get; set; }
        public OrderHeaderModel OrderHeader { get; set; }
        public ProductModel Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
