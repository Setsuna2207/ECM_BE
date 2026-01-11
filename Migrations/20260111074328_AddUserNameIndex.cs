using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddUserNameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add index on UserName for faster lookups (if not exists)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_UserName' AND object_id = OBJECT_ID('AspNetUsers'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_UserName 
                    ON AspNetUsers(UserName) 
                    WHERE UserName IS NOT NULL;
                END
            ");

            // Add index on Email for faster lookups (if not exists)
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_Email' AND object_id = OBJECT_ID('AspNetUsers'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX IX_AspNetUsers_Email 
                    ON AspNetUsers(Email) 
                    WHERE Email IS NOT NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop indexes if they exist
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_UserName' AND object_id = OBJECT_ID('AspNetUsers'))
                BEGIN
                    DROP INDEX IX_AspNetUsers_UserName ON AspNetUsers;
                END
            ");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AspNetUsers_Email' AND object_id = OBJECT_ID('AspNetUsers'))
                BEGIN
                    DROP INDEX IX_AspNetUsers_Email ON AspNetUsers;
                END
            ");
        }
    }
}
