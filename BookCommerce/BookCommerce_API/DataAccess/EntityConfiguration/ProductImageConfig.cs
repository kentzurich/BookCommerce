using BookCommerce_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace BookCommerce_API.DataAccess.EntityConfiguration
{
    public class ProductImageConfig : IEntityTypeConfiguration<ProductImageModel>
    {
        public void Configure(EntityTypeBuilder<ProductImageModel> builder)
        {
            SeedEntity(builder);
        }

        private void SeedEntity(EntityTypeBuilder<ProductImageModel> builder)
        {
            builder.HasData(
                new ProductImageModel
                {
                    ProductImageId = 1,
                    ImgUrl = "https://placehold.co/500x600/png",
                    ImgLocalPath = "https://placehold.co/500x600/png",
                    ProductId = 1,
                },
                new ProductImageModel
                {
                    ProductImageId = 2,
                    ImgUrl = "https://placehold.co/500x600/png",
                    ImgLocalPath = "https://placehold.co/500x600/png",
                    ProductId = 2,
                },
                new ProductImageModel
                {
                    ProductImageId = 3,
                    ImgUrl = "https://placehold.co/500x600/png",
                    ImgLocalPath = "https://placehold.co/500x600/png",
                    ProductId = 3,
                },
                new ProductImageModel
                {
                    ProductImageId = 4,
                    ImgUrl = "https://placehold.co/500x600/png",
                    ImgLocalPath = "https://placehold.co/500x600/png",
                    ProductId = 4,
                },
                new ProductImageModel
                {
                    ProductImageId = 5,
                    ImgUrl = "https://placehold.co/500x600/png",
                    ImgLocalPath = "https://placehold.co/500x600/png",
                    ProductId = 5,
                },
                new ProductImageModel
                {
                    ProductImageId = 6,
                    ImgUrl = "https://placehold.co/500x600/png",
                    ImgLocalPath = "https://placehold.co/500x600/png",
                    ProductId = 6,
                });
        }
    }
}
