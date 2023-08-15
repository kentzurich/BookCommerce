using System.ComponentModel.DataAnnotations;

namespace BookCommerce_API.Models.DTO.Product
{
    public class UpdateProductDTO
    {
        [Required]
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
        [Required]
        public int CoverTypeId { get; set; }
        public List<IFormFile> Image { get; set; }
    }
}
