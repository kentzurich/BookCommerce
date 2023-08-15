namespace BookCommerce_WEB.Models.DTO.ProductImage
{
    public class ProductImageDTO
    {
        public int ProductImageId { get; set; }
        public int ProductId { get; set; }
        public string ImgUrl { get; set; }
        public string ImgLocalPath { get; set; }
    }
}
