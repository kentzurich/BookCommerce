using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models
{
    public class ProductImageModel
    {
        [Key]
        public int ProductImageId { get; set; }
        [Required]
        public string ImgUrl { get; set; }
        public string ImgLocalPath { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        [ValidateNever]
        public ProductModel Product { get; set; }
    }
}
