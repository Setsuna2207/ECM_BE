using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLearningPathAndAIFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AIFeedback");

            migrationBuilder.DropTable(
                name: "LearningPaths");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AIFeedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ResultID = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    FeedbackSummary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RcmCourses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeakSkill = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AIFeedback", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK_AIFeedbacks_TestResults",
                        column: x => x.ResultID,
                        principalTable: "TestResults",
                        principalColumn: "ResultID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningPaths",
                columns: table => new
                {
                    LearningPathID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FinalResultID = table.Column<int>(type: "int", nullable: true),
                    FinalTestID = table.Column<int>(type: "int", nullable: true),
                    InitialResultID = table.Column<int>(type: "int", nullable: true),
                    InitialTestID = table.Column<int>(type: "int", nullable: true),
                    UserGoalID = table.Column<int>(type: "int", nullable: false),
                    userID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CompletedCourses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "getdate()"),
                    RecommendedCourses = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "GoalSet"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningPaths", x => x.LearningPathID);
                    table.ForeignKey(
                        name: "FK_LearningPaths_FinalResult",
                        column: x => x.FinalResultID,
                        principalTable: "TestResults",
                        principalColumn: "ResultID");
                    table.ForeignKey(
                        name: "FK_LearningPaths_FinalTest",
                        column: x => x.FinalTestID,
                        principalTable: "PlacementTests",
                        principalColumn: "TestID");
                    table.ForeignKey(
                        name: "FK_LearningPaths_InitialResult",
                        column: x => x.InitialResultID,
                        principalTable: "TestResults",
                        principalColumn: "ResultID");
                    table.ForeignKey(
                        name: "FK_LearningPaths_InitialTest",
                        column: x => x.InitialTestID,
                        principalTable: "PlacementTests",
                        principalColumn: "TestID");
                    table.ForeignKey(
                        name: "FK_LearningPaths_UserGoals",
                        column: x => x.UserGoalID,
                        principalTable: "UserGoals",
                        principalColumn: "UserGoalID");
                    table.ForeignKey(
                        name: "FK_LearningPaths_Users",
                        column: x => x.userID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AIFeedback_ResultID",
                table: "AIFeedback",
                column: "ResultID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_FinalResultID",
                table: "LearningPaths",
                column: "FinalResultID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_FinalTestID",
                table: "LearningPaths",
                column: "FinalTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_InitialResultID",
                table: "LearningPaths",
                column: "InitialResultID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_InitialTestID",
                table: "LearningPaths",
                column: "InitialTestID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_UserGoalID",
                table: "LearningPaths",
                column: "UserGoalID");

            migrationBuilder.CreateIndex(
                name: "IX_LearningPaths_userID",
                table: "LearningPaths",
                column: "userID");
        }
    }
}
