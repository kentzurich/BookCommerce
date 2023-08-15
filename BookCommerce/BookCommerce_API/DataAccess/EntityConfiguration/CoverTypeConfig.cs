using BookCommerce_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookCommerce_API.DataAccess.EntityConfiguration
{
    public class CoverTypeConfig : IEntityTypeConfiguration<CoverTypeModel>
    {
        public void Configure(EntityTypeBuilder<CoverTypeModel> builder)
        {
            SeedEntity(builder);
        }

        private void SeedEntity(EntityTypeBuilder<CoverTypeModel> builder)
        {
            builder.HasData(
                new CoverTypeModel
                {
                    CoverTypeId = 1,
                    Name = "Hard Cover"
                },
                new CoverTypeModel
                {
                    CoverTypeId = 2,
                    Name = "Soft Cover"
                });
        }
    }
}
