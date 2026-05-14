using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Practical_Assignment001.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: "AC001");

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: "AC002");

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: "AC003");

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "FailedAttempts", "FullName", "IsLocked", "PIN" },
                values: new object[,]
                {
                    { "STU001", 0, "Alice Mutoni", false, "1234" },
                    { "STU002", 0, "Bob Nkurunziza", false, "5678" },
                    { "STU003", 0, "Claire Uwase", false, "9999" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: "STU001");

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: "STU002");

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: "STU003");

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "FailedAttempts", "FullName", "IsLocked", "PIN" },
                values: new object[,]
                {
                    { "AC001", 0, "Alice Mutoni", false, "1234" },
                    { "AC002", 0, "Bob Nkurunziza", false, "5678" },
                    { "AC003", 0, "Claire Uwase", false, "9999" }
                });
        }
    }
}
