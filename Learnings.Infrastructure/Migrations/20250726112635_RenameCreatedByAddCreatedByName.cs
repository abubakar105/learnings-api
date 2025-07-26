using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCreatedByAddCreatedByName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Reviews",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "ProductsAttribute",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "ProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "ProductAttributes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Carts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "CartItems",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "ProductsAttribute");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "ProductImages");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "ProductAttributes");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "CartItems");
        }
    }
}
