using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddAIRcmTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIRcms",
                columns: table => new
                {
                    RcmID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RcmCourses = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIRcms", x => x.RcmID);
                    table.ForeignKey(
                        name: "FK_AIRcms_Users",
                        column: x => x.userID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIRcms_userID",
                table: "AIRcms",
                column: "userID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIRcms");
        }
    }
}
