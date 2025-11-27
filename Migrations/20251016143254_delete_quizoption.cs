using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class delete_quizoption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizResults_QuizOptions",
                table: "QuizResults");

            migrationBuilder.DropTable(
                name: "QuizOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuizResults_SelectedOptionID",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "SelectedOptionID",
                table: "QuizResults");

            migrationBuilder.RenameColumn(
                name: "TakenAt",
                table: "QuizResults",
                newName: "SubmittedAt");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "QuizResults",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalQuestions",
                table: "QuizResults",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAnswers",
                table: "QuizResults",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "TotalQuestions",
                table: "QuizResults");

            migrationBuilder.DropColumn(
                name: "UserAnswers",
                table: "QuizResults");

            migrationBuilder.RenameColumn(
                name: "SubmittedAt",
                table: "QuizResults",
                newName: "TakenAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "QuizResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SelectedOptionID",
                table: "QuizResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "QuizOptions",
                columns: table => new
                {
                    OptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuizID = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizOptions", x => x.OptionID);
                    table.ForeignKey(
                        name: "FK_QuizOptions_Quizzes",
                        column: x => x.QuizID,
                        principalTable: "Quizzes",
                        principalColumn: "QuizID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuizResults_SelectedOptionID",
                table: "QuizResults",
                column: "SelectedOptionID");

            migrationBuilder.CreateIndex(
                name: "IX_QuizOptions_QuizID",
                table: "QuizOptions",
                column: "QuizID");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizResults_QuizOptions",
                table: "QuizResults",
                column: "SelectedOptionID",
                principalTable: "QuizOptions",
                principalColumn: "OptionID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
