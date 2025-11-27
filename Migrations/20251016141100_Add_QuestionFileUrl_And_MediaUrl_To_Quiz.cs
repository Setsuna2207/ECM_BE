using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class Add_QuestionFileUrl_And_MediaUrl_To_Quiz : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Question",
                table: "Quizzes");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MediaUrl",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "QuestionFileUrl",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "MediaUrl",
                table: "Quizzes");

            migrationBuilder.DropColumn(
                name: "QuestionFileUrl",
                table: "Quizzes");

            migrationBuilder.AddColumn<string>(
                name: "Question",
                table: "Quizzes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
