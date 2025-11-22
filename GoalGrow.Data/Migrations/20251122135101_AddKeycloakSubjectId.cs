using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoalGrow.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddKeycloakSubjectId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KeycloakSubjectId",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KeycloakSubjectId",
                table: "Users");
        }
    }
}
