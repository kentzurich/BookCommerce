using BookCommerce_API.Models.DTO.ApplicationUser;
using BookCommerce_API.Models.DTO.Product;

namespace BookCommerce_API.Models.DTO.ShoppingCart
{
    public class ShoppingCartDTO
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public ProductDTO Product { get; set; }
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUserDTO ApplicationUser { get; set; }
        public double Price { get; set; }

    }
}
