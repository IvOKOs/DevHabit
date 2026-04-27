using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevHabit.Api.Migrations.Application;

/// <inheritdoc />
public partial class Add_UserId_Reference : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
     
    DELETE FROM devhabit.HabitTags;
    DELETE FROM devhabit.Habits; 
    DELETE FROM devhabit.Tags;
    """);

        migrationBuilder.DropIndex(
            name: "IX_Tags_Name",
            schema: "devhabit",
            table: "Tags");

        migrationBuilder.AddColumn<string>(
            name: "UserId",
            schema: "devhabit",
            table: "Tags",
            type: "nvarchar(500)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "UserId",
            schema: "devhabit",
            table: "Habits",
            type: "nvarchar(500)",
            nullable: false,
            defaultValue: "");

#pragma warning disable CA1861 // Avoid constant arrays as arguments
        migrationBuilder.CreateIndex(
            name: "IX_Tags_UserId_Name",
            schema: "devhabit",
            table: "Tags",
            columns: new[] { "UserId", "Name" },
            unique: true);
#pragma warning restore CA1861 // Avoid constant arrays as arguments

        migrationBuilder.CreateIndex(
            name: "IX_Habits_UserId",
            schema: "devhabit",
            table: "Habits",
            column: "UserId");

        migrationBuilder.AddForeignKey(
            name: "FK_Habits_Users_UserId",
            schema: "devhabit",
            table: "Habits",
            column: "UserId",
            principalSchema: "devhabit",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Tags_Users_UserId",
            schema: "devhabit",
            table: "Tags",
            column: "UserId",
            principalSchema: "devhabit",
            principalTable: "Users",
            principalColumn: "Id");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Habits_Users_UserId",
            schema: "devhabit",
            table: "Habits");

        migrationBuilder.DropForeignKey(
            name: "FK_Tags_Users_UserId",
            schema: "devhabit",
            table: "Tags");

        migrationBuilder.DropIndex(
            name: "IX_Tags_UserId_Name",
            schema: "devhabit",
            table: "Tags");

        migrationBuilder.DropIndex(
            name: "IX_Habits_UserId",
            schema: "devhabit",
            table: "Habits");

        migrationBuilder.DropColumn(
            name: "UserId",
            schema: "devhabit",
            table: "Tags");

        migrationBuilder.DropColumn(
            name: "UserId",
            schema: "devhabit",
            table: "Habits");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name",
            schema: "devhabit",
            table: "Tags",
            column: "Name",
            unique: true);
    }
}
