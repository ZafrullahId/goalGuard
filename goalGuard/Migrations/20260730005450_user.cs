using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace goalGuard.Migrations
{
    /// <inheritdoc />
    public partial class user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    BmoniUserId = table.Column<string>(type: "text", nullable: true),
                    OwnerAddress = table.Column<string>(type: "text", nullable: true),
                    OwnerPrivateKey = table.Column<string>(type: "text", nullable: true),
                    SmartWalletId = table.Column<string>(type: "text", nullable: true),
                    SmartWalletAddress = table.Column<string>(type: "text", nullable: true),
                    Bvn = table.Column<string>(type: "text", nullable: true),
                    NigeriaRailActive = table.Column<bool>(type: "boolean", nullable: false),
                    WalletFunded = table.Column<bool>(type: "boolean", nullable: false),
                    OnboardingStatus = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_BmoniUserId",
                table: "Users",
                column: "BmoniUserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
