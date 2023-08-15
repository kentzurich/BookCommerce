using BookCommerce_WEB.Models.DTO.Order.Header;
using BookCommerce_WEB.Models.DTO.Product;

namespace BookCommerce_WEB.Models.DTO.Order.Details
{
    public class OrderDetailsDTO
    {
        public int OrderDetailsId { get; set; }
        public OrderHeaderDTO OrderHeader { get; set; }
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
