﻿// <auto-generated />
using System;
using BookCommerce_API.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BookCommerce_API.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20230613091450_addOneToMany_Product_CoverType_Product_Category")]
    partial class addOneToMany_Product_CoverType_Product_Category
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BookCommerce_API.Model.CategoryModel", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CategoryId"));

                    b.Property<DateTime>("DateTimeCreated")
                        .HasColumnType("datetime2");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(30)
                        .HasColumnType("nvarchar(30)");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            DateTimeCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DisplayOrder = 1,
                            Name = "Action"
                        },
                        new
                        {
                            CategoryId = 2,
                            DateTimeCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DisplayOrder = 2,
                            Name = "Horror"
                        },
                        new
                        {
                            CategoryId = 3,
                            DateTimeCreated = new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                            DisplayOrder = 4,
                            Name = "SciFi"
                        });
                });

            modelBuilder.Entity("BookCommerce_API.Model.CompanyModel", b =>
                {
                    b.Property<int>("CompanyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CompanyId"));

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PostalCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StreetAddress")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CompanyId");

                    b.ToTable("Companies");
                });

            modelBuilder.Entity("BookCommerce_API.Model.CoverTypeModel", b =>
                {
                    b.Property<int>("CoverTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CoverTypeId"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("CoverTypeId");

                    b.ToTable("CoverTypes");

                    b.HasData(
                        new
                        {
                            CoverTypeId = 1,
                            Name = "Hard Cover"
                        },
                        new
                        {
                            CoverTypeId = 2,
                            Name = "Soft Cover"
                        });
                });

            modelBuilder.Entity("BookCommerce_API.Model.ProductModel", b =>
                {
                    b.Property<int>("ProductId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProductId"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("CoverTypeId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ListPrice")
                        .HasColumnType("float");

                    b.Property<double>("Price")
                        .HasColumnType("float");

                    b.Property<double>("Price_100")
                        .HasColumnType("float");

                    b.Property<double>("Price_50")
                        .HasColumnType("float");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ProductId");

                    b.HasIndex("CategoryId");

                    b.HasIndex("CoverTypeId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            ProductId = 1,
                            Author = "Billy Spark",
                            CategoryId = 1,
                            CoverTypeId = 2,
                            Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                            ISBN = "SWD9999001",
                            ListPrice = 99.0,
                            Price = 90.0,
                            Price_100 = 80.0,
                            Price_50 = 85.0,
                            Title = "Fortune of Time"
                        },
                        new
                        {
                            ProductId = 2,
                            Author = "Nancy Hoover",
                            CategoryId = 1,
                            CoverTypeId = 2,
                            Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                            ISBN = "CAW777777701",
                            ListPrice = 40.0,
                            Price = 30.0,
                            Price_100 = 20.0,
                            Price_50 = 25.0,
                            Title = "Dark Skies"
                        },
                        new
                        {
                            ProductId = 3,
                            Author = "Julian Button",
                            CategoryId = 1,
                            CoverTypeId = 2,
                            Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                            ISBN = "RITO5555501",
                            ListPrice = 55.0,
                            Price = 50.0,
                            Price_100 = 35.0,
                            Price_50 = 40.0,
                            Title = "Vanish in the Sunset"
                        },
                        new
                        {
                            ProductId = 4,
                            Author = "Abby Muscles",
                            CategoryId = 1,
                            CoverTypeId = 2,
                            Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                            ISBN = "WS3333333301",
                            ListPrice = 70.0,
                            Price = 65.0,
                            Price_100 = 55.0,
                            Price_50 = 60.0,
                            Title = "Cotton Candy"
                        },
                        new
                        {
                            ProductId = 5,
                            Author = "Ron Parker",
                            CategoryId = 1,
                            CoverTypeId = 2,
                            Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                            ISBN = "SOTJ1111111101",
                            ListPrice = 30.0,
                            Price = 27.0,
                            Price_100 = 20.0,
                            Price_50 = 25.0,
                            Title = "Rock in the Ocean"
                        },
                        new
                        {
                            ProductId = 6,
                            Author = "Laura Phantom",
                            CategoryId = 1,
                            CoverTypeId = 2,
                            Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
                            ISBN = "FOT000000001",
                            ListPrice = 25.0,
                            Price = 23.0,
                            Price_100 = 20.0,
                            Price_50 = 22.0,
                            Title = "Leaves and Wonders"
                        });
                });

            modelBuilder.Entity("BookCommerce_API.Model.ProductModel", b =>
                {
                    b.HasOne("BookCommerce_API.Model.CategoryModel", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BookCommerce_API.Model.CoverTypeModel", "CoverType")
                        .WithMany()
                        .HasForeignKey("CoverTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");

                    b.Navigation("CoverType");
                });
#pragma warning restore 612, 618
        }
    }
}
