using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookCommerce_API.Models
{
    public class ProductModel
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        [Range(1, 10000)]
        public double ListPrice { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price_50 { get; set; }

        [Required]
        [Range(1, 10000)]
        public double Price_100 { get; set; }

        [Required]
        public int CategoryId { get; set; }
        [ForeignKey(nameof(CategoryId))]
        [ValidateNever]
        public CategoryModel Category { get; set; }

        [Required]
        public int CoverTypeId { get; set; }
        [ForeignKey(nameof(CoverTypeId))]
        [ValidateNever]
        public CoverTypeModel CoverType { get; set; }

        [ValidateNever]
        public List<ProductImageModel> ProductImage { get; set; }
    }
}
