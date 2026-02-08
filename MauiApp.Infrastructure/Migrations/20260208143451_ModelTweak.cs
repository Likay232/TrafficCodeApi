using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MauiApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelTweak : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AmountToLevelUp",
                table: "progresses",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "completed_tasks",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_tasks_ThemeId",
                table: "tasks",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_lessons_ThemeId",
                table: "lessons",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_completed_tasks_TaskId",
                table: "completed_tasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_completed_tasks_tasks_TaskId",
                table: "completed_tasks",
                column: "TaskId",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_lessons_themes_ThemeId",
                table: "lessons",
                column: "ThemeId",
                principalTable: "themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_themes_ThemeId",
                table: "tasks",
                column: "ThemeId",
                principalTable: "themes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_completed_tasks_tasks_TaskId",
                table: "completed_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_lessons_themes_ThemeId",
                table: "lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_themes_ThemeId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_ThemeId",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_lessons_ThemeId",
                table: "lessons");

            migrationBuilder.DropIndex(
                name: "IX_completed_tasks_TaskId",
                table: "completed_tasks");

            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "completed_tasks");

            migrationBuilder.AlterColumn<int>(
                name: "AmountToLevelUp",
                table: "progresses",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");
        }
    }
}
