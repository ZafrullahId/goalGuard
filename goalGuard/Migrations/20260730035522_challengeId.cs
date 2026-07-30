using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace goalGuard.Migrations
{
    /// <inheritdoc />
    public partial class challengeId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PendingChallengeExpiresAt",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingChallengeId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PendingChallengeMessage",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendingChallengeExpiresAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PendingChallengeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PendingChallengeMessage",
                table: "Users");
        }
    }
}
