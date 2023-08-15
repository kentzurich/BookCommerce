using BookCommerce_WEB.Models.DTO.Order.Header;
using BookCommerce_WEB.Models.DTO.ShoppingCart;

namespace BookCommerce_WEB.Models.ViewModel
{
    public class ShoppingCartViewModel
    {
        public IEnumerable<ShoppingCartDTO> ListCart { get; set; }
        public OrderHeaderDTO OrderHeader { get; set; }
    }
}
