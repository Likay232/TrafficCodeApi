using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddModifiedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "users");

            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "progresses");

            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "completed_tasks");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "RegistrationDate",
                table: "users",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "themes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "progresses",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "lessons",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "completed_tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "exchange_history",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExchangedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_exchange_history", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exchange_history");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "users");

            migrationBuilder.DropColumn(
                name: "RegistrationDate",
                table: "users");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "themes");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "progresses");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "lessons");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "completed_tasks");

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "progresses",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "completed_tasks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
