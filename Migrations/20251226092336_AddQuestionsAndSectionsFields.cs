using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionsAndSectionsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Questions",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sections",
                table: "PlacementTests",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Questions",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "Sections",
                table: "PlacementTests");
        }
    }
}
