using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class removecategoryfromtest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_PlacementTests_PlacementTestTestID",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_PlacementTestTestID",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "PlacementTestTestID",
                table: "Categories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlacementTestTestID",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_PlacementTestTestID",
                table: "Categories",
                column: "PlacementTestTestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_PlacementTests_PlacementTestTestID",
                table: "Categories",
                column: "PlacementTestTestID",
                principalTable: "PlacementTests",
                principalColumn: "TestID");
        }
    }
}
