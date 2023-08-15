using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models.DTO.ShoppingCart
{
    public class CreateShoppingCartDTO
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
        public double Price { get; set; }
    }
}
