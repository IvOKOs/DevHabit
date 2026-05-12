using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class AddEntries : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "AutomationSource",
            schema: "devhabit",
            table: "Habits",
            type: "int",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "Entries",
            schema: "devhabit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                HabitId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                UserId = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Value = table.Column<int>(type: "int", nullable: false),
                Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                Source = table.Column<int>(type: "int", nullable: false),
                ExternalId = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                IsArchived = table.Column<bool>(type: "bit", nullable: false),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Entries", x => x.Id);
                table.ForeignKey(
                    name: "FK_Entries_Habits_HabitId",
                    column: x => x.HabitId,
                    principalSchema: "devhabit",
                    principalTable: "Habits",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Entries_Users_UserId",
                    column: x => x.UserId,
                    principalSchema: "devhabit",
                    principalTable: "Users",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_Entries_ExternalId",
            schema: "devhabit",
            table: "Entries",
            column: "ExternalId",
            unique: true,
            filter: "[ExternalId] IS NOT NULL");

        migrationBuilder.CreateIndex(
            name: "IX_Entries_HabitId",
            schema: "devhabit",
            table: "Entries",
            column: "HabitId");

        migrationBuilder.CreateIndex(
            name: "IX_Entries_UserId",
            schema: "devhabit",
            table: "Entries",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Entries",
            schema: "devhabit");

        migrationBuilder.DropColumn(
            name: "AutomationSource",
            schema: "devhabit",
            table: "Habits");
    }
}
