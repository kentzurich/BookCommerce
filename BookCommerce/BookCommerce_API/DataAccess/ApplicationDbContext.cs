using BookCommerce_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace BookCommerce_API.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {    
        }

        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CoverTypeModel> CoverTypes { get; set; }
        public DbSet<CompanyModel> Companies { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<ProductImageModel> ProductImages { get; set; }
        public DbSet<ApplicationUserModel> ApplicationUsers { get; set; }
        public DbSet<OrderHeaderModel> OrderHeaders { get; set; }
        public DbSet<OrderDetailsModel> OrderDetails { get; set; }
        public DbSet<ShoppingCartModel> ShoppingCarts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
