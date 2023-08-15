namespace BookCommerce_WEB.Models.DTO.Product
{
    public class UpdateProductDTO
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
        public int CategoryId { get; set; }
        public int CoverTypeId { get; set; }
        public List<IFormFile> Image { get; set; }
    }
}
