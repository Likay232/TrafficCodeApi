using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_test_users_UserId",
                table: "test_users",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_test_users_users_UserId",
                table: "test_users",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_users_users_UserId",
                table: "test_users");

            migrationBuilder.DropIndex(
                name: "IX_test_users_UserId",
                table: "test_users");
        }
    }
}
