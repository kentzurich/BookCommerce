namespace BookCommerce_API.Models.DTO.ProductImage
{
    public class CreateProductImageDTO
    {
        public string ImgUrl { get; set; }
        public string ImgLocalPath { get; set; }
        public int ProductId { get; set; }
    }
}
