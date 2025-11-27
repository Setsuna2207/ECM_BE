using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECM_BE.Migrations
{
    /// <inheritdoc />
    public partial class remove_type_category_and_change_usergoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGoals_Categories",
                table: "UserGoals");

            migrationBuilder.DropIndex(
                name: "IX_UserGoals_CategoryID",
                table: "UserGoals");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "CategoryUserGoal",
                columns: table => new
                {
                    CategoriesCategoryID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryUserGoal", x => new { x.CategoriesCategoryID, x.CategoryID });
                });

            migrationBuilder.CreateTable(
                name: "UserGoalsCategories",
                columns: table => new
                {
                    UserGoalID = table.Column<int>(type: "int", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGoalsCategories", x => new { x.UserGoalID, x.CategoryID });
                    table.ForeignKey(
                        name: "FK_UserGoalsCategories_Categories",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserGoalsCategories_UserGoals",
                        column: x => x.UserGoalID,
                        principalTable: "UserGoals",
                        principalColumn: "UserGoalID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserGoalsCategories_CategoryID",
                table: "UserGoalsCategories",
                column: "CategoryID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryUserGoal");

            migrationBuilder.DropTable(
                name: "UserGoalsCategories");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Categories",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_UserGoals_CategoryID",
                table: "UserGoals",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_UserGoals_Categories",
                table: "UserGoals",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
