using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.Order.Details
{
    public class CreateOrderDetailsDTO
    {
        public int OrderHeaderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
