using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookCommerce_API.Migrations
{
    /// <inheritdoc />
    public partial class addCategoryToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    DateTimeCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryId", "DateTimeCreated", "DisplayOrder", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 6, 12, 13, 6, 48, 654, DateTimeKind.Utc).AddTicks(4549), 1, "Action" },
                    { 2, new DateTime(2023, 6, 12, 13, 6, 48, 654, DateTimeKind.Utc).AddTicks(4554), 2, "Horror" },
                    { 3, new DateTime(2023, 6, 12, 13, 6, 48, 654, DateTimeKind.Utc).AddTicks(4555), 4, "SciFi" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
