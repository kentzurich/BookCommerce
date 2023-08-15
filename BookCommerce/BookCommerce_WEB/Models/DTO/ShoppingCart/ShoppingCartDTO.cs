using BookCommerce_WEB.Models.DTO.ApplicationUser;
using BookCommerce_WEB.Models.DTO.Product;

namespace BookCommerce_WEB.Models.DTO.ShoppingCart
{
    public class ShoppingCartDTO
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
        public ApplicationUserDTO ApplicationUser { get; set; }
        public string AppllicationUserId { get; set; }
        public double Price { get; set; }

    }
}
