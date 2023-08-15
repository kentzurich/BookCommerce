using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models
{
    public class OrderDetailsModel
    {
        [Key]
        public int OrderDetailsId { get; set; }
        [Required]
        public int OrderHeaderId { get; set; }
        [ForeignKey(nameof(OrderHeaderId))]
        [ValidateNever]
        public OrderHeaderModel OrderHeader { get; set; }
        [Required]
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public ProductModel Product { get; set; }
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
