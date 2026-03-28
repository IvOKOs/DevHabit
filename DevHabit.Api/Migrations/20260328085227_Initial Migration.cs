// >auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations;

/// <inheritdoc />
public partial class InitialMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "devhabit");

        migrationBuilder.CreateTable(
            name: "Habits",
            schema: "devhabit",
            columns: table => new
            {
                Id = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Type = table.Column<int>(type: "int", nullable: false),
                Frequency_Type = table.Column<int>(type: "int", nullable: false),
                Frequency_TimesPerPeriod = table.Column<int>(type: "int", nullable: false),
                Target_Value = table.Column<int>(type: "int", nullable: false),
                Target_Unit = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                IsArchived = table.Column<bool>(type: "bit", nullable: false),
                EndDate = table.Column<DateOnly>(type: "date", nullable: true),
                MilestoneTarget = table.Column<int>(type: "int", nullable: true),
                MilestoneCurrent = table.Column<int>(type: "int", nullable: true),
                CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                LastCompletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Habits", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Habits",
            schema: "devhabit");
    }
}
