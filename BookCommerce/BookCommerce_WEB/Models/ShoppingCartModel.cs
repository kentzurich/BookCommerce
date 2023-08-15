using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookCommerce_WEB.Models
{
    public class ShoppingCartModel
    {
        [Key]
        public int CartId { get; set; }

        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public ProductModel Product { get; set; }
        public int Count { get; set; }

        public string ApplicationUserId { get; set; }
        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUserModel ApplicationUser { get; set; }
    }
}
