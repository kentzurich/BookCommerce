using BookCommerce_WEB.Models.DTO.Category;
using BookCommerce_WEB.Models.DTO.CoverType;
using BookCommerce_WEB.Models.DTO.ProductImage;

namespace BookCommerce_WEB.Models.DTO.Product
{
    public class ProductDTO
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ISBN { get; set; }
        public string Author { get; set; }
        public double ListPrice { get; set; }
        public double Price { get; set; }
        public double Price_50 { get; set; }
        public double Price_100 { get; set; }
        public CategoryDTO Category { get; set; }
        public CoverTypeDTO CoverType { get; set; }
        public List<ProductImageDTO> ProductImage { get; set; }
    }
}
