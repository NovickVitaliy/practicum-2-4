using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nimble.Modulith.Reporting.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSeparateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Reporting");

            migrationBuilder.RenameTable(
                name: "FactOrders",
                newName: "FactOrders",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "DimProduct",
                newName: "DimProduct",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "DimDate",
                newName: "DimDate",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "DimCustomer",
                newName: "DimCustomer",
                newSchema: "Reporting");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "FactOrders",
                schema: "Reporting",
                newName: "FactOrders");

            migrationBuilder.RenameTable(
                name: "DimProduct",
                schema: "Reporting",
                newName: "DimProduct");

            migrationBuilder.RenameTable(
                name: "DimDate",
                schema: "Reporting",
                newName: "DimDate");

            migrationBuilder.RenameTable(
                name: "DimCustomer",
                schema: "Reporting",
                newName: "DimCustomer");
        }
    }
}
