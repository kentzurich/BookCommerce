using BookCommerce_API.Models.DTO.Order.Header;
using BookCommerce_API.Models.DTO.ShoppingCart;

namespace BookCommerce_API.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCartDTO> ListCart { get; set; }
        public OrderHeaderDTO OrderHeader { get; set; }
    }
}
