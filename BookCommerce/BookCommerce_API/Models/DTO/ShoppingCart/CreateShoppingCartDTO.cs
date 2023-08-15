using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.ShoppingCart
{
    public class CreateShoppingCartDTO
    {
        public int ProductId { get; set; }
        public int Count { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
