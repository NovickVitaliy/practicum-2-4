using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nimble.Modulith.Users.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRolesWithCorrectIdGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Users",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0049d578-28f3-4bf5-9803-3fef558301cd");

            migrationBuilder.DeleteData(
                schema: "Users",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "24d1b0c3-eadb-460c-93f3-906679c2a0df");

            migrationBuilder.InsertData(
                schema: "Users",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1E603235-6183-4E4D-8FF3-88768D8A9D80", "624221AF-31EA-489A-A8E8-7DA2C13877E4", "Admin", "ADMIN" },
                    { "5BE9B034-D673-40F0-B59E-B6621D135979", "E5FD9B42-11BA-4060-A862-2584BA5FD741", "Customer", "CUSTOMER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Users",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1E603235-6183-4E4D-8FF3-88768D8A9D80");

            migrationBuilder.DeleteData(
                schema: "Users",
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5BE9B034-D673-40F0-B59E-B6621D135979");

            migrationBuilder.InsertData(
                schema: "Users",
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0049d578-28f3-4bf5-9803-3fef558301cd", "60f67c44-9840-489a-b475-5be62b48d92b", "Admin", "ADMIN" },
                    { "24d1b0c3-eadb-460c-93f3-906679c2a0df", "9dbbe9c3-f581-4c24-9d28-a8983305bc20", "Customer", "CUSTOMER" }
                });
        }
    }
}
