using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedDescInPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PermissionDescription",
                table: "Permissions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PermissionDescription",
                table: "Permissions");
        }
    }
}
