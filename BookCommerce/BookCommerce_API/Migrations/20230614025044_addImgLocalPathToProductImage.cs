using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookCommerce_API.Migrations
{
    /// <inheritdoc />
    public partial class addImgLocalPathToProductImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImgLocalPath",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ProductImageId",
                keyValue: 1,
                column: "ImgLocalPath",
                value: "https://placehold.co/500x600/png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ProductImageId",
                keyValue: 2,
                column: "ImgLocalPath",
                value: "https://placehold.co/500x600/png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ProductImageId",
                keyValue: 3,
                column: "ImgLocalPath",
                value: "https://placehold.co/500x600/png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ProductImageId",
                keyValue: 4,
                column: "ImgLocalPath",
                value: "https://placehold.co/500x600/png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ProductImageId",
                keyValue: 5,
                column: "ImgLocalPath",
                value: "https://placehold.co/500x600/png");

            migrationBuilder.UpdateData(
                table: "ProductImages",
                keyColumn: "ProductImageId",
                keyValue: 6,
                column: "ImgLocalPath",
                value: "https://placehold.co/500x600/png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgLocalPath",
                table: "ProductImages");
        }
    }
}
