using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Learnings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakePermissionIdIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        -- 1) Copy existing data to a temp table
        SELECT PermissionId, PermissionName, PermissionDescription
          INTO dbo.Permissions_Tmp
          FROM dbo.Permissions;

        -- 2) Drop the old table
        DROP TABLE dbo.Permissions;

        -- 3) Recreate with IDENTITY on PermissionId
        CREATE TABLE dbo.Permissions
        (
          PermissionId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
          PermissionName NVARCHAR(255) NOT NULL,
          PermissionDescription NVARCHAR(255) NOT NULL
        );

        -- 4) Preserve existing keys
        SET IDENTITY_INSERT dbo.Permissions ON;
        INSERT INTO dbo.Permissions (PermissionId, PermissionName, PermissionDescription)
          SELECT PermissionId, PermissionName, PermissionDescription
          FROM dbo.Permissions_Tmp;
        SET IDENTITY_INSERT dbo.Permissions OFF;

        -- 5) Clean up
        DROP TABLE dbo.Permissions_Tmp;
    ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        -- reverse Up: move data out, drop, recreate without IDENTITY, re-insert
        SELECT PermissionId, PermissionName, PermissionDescription
          INTO dbo.Permissions_Tmp
          FROM dbo.Permissions;
        DROP TABLE dbo.Permissions;
        CREATE TABLE dbo.Permissions
        (
          PermissionId INT NOT NULL PRIMARY KEY,
          PermissionName NVARCHAR(255) NOT NULL,
          PermissionDescription NVARCHAR(255) NOT NULL
        );
        INSERT INTO dbo.Permissions (PermissionId, PermissionName, PermissionDescription)
          SELECT PermissionId, PermissionName, PermissionDescription
          FROM dbo.Permissions_Tmp;
        DROP TABLE dbo.Permissions_Tmp;
    ");
        }

    }
}
