using BookCommerce_WEB.Models.DTO.Order.Details;
using BookCommerce_WEB.Models.DTO.Order.Header;

namespace BookCommerce_WEB.Models.ViewModel
{
    public class OrderViewModel
    {
        public OrderHeaderDTO OrderHeader { get; set; }
        public IEnumerable<OrderDetailsDTO> OrderDetails { get; set; }
    }
}
