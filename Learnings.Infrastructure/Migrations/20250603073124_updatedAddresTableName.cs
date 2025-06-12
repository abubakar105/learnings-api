using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedAddresTableName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_AspNetUsers_CreatedBy",
                table: "UserAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_AspNetUsers_UpdatedBy",
                table: "UserAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddress_AspNetUsers_UserId",
                table: "UserAddress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAddress",
                table: "UserAddress");

            migrationBuilder.RenameTable(
                name: "UserAddress",
                newName: "UserAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddress_UserId",
                table: "UserAddresses",
                newName: "IX_UserAddresses_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddress_UpdatedBy",
                table: "UserAddresses",
                newName: "IX_UserAddresses_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddress_CreatedBy",
                table: "UserAddresses",
                newName: "IX_UserAddresses_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAddresses",
                table: "UserAddresses",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_AspNetUsers_CreatedBy",
                table: "UserAddresses",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UpdatedBy",
                table: "UserAddresses",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId",
                table: "UserAddresses",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_AspNetUsers_CreatedBy",
                table: "UserAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UpdatedBy",
                table: "UserAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_AspNetUsers_UserId",
                table: "UserAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAddresses",
                table: "UserAddresses");

            migrationBuilder.RenameTable(
                name: "UserAddresses",
                newName: "UserAddress");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddresses_UserId",
                table: "UserAddress",
                newName: "IX_UserAddress_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddresses_UpdatedBy",
                table: "UserAddress",
                newName: "IX_UserAddress_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_UserAddresses_CreatedBy",
                table: "UserAddress",
                newName: "IX_UserAddress_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAddress",
                table: "UserAddress",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_AspNetUsers_CreatedBy",
                table: "UserAddress",
                column: "CreatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_AspNetUsers_UpdatedBy",
                table: "UserAddress",
                column: "UpdatedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAddress_AspNetUsers_UserId",
                table: "UserAddress",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
