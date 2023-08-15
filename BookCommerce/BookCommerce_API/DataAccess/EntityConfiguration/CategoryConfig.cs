using BookCommerce_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCommerce_API.DataAccess.EntityConfiguration
{
    public class CategoryConfig : IEntityTypeConfiguration<CategoryModel>
    {
        public void Configure(EntityTypeBuilder<CategoryModel> builder)
        {
            SeedEntity(builder);
        }

        private void SeedEntity(EntityTypeBuilder<CategoryModel> builder)
        {
            builder.HasData(
                new CategoryModel
                {
                    CategoryId = 1,
                    Name = "Action",
                    DisplayOrder = 1,
                },
                new CategoryModel
                {
                    CategoryId = 2,
                    Name = "Horror",
                    DisplayOrder = 2,
                },
                new CategoryModel
                {
                    CategoryId = 3,
                    Name = "SciFi",
                    DisplayOrder = 4,
                });
        }
    }
}
