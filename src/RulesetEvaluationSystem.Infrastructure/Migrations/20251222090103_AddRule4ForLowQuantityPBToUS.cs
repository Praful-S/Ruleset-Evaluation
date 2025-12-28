using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RulesetEvaluationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRule4ForLowQuantityPBToUS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "tbl_Rule",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Priority", "ProductionPlant", "RulesetId", "UpdatedAt" },
                values: new object[] { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PB books with low quantity to US for Publisher 99999", true, "Rule 4", 3, "US", 2, null });

            migrationBuilder.InsertData(
                table: "tbl_RuleCondition",
                columns: new[] { "Id", "CreatedAt", "Field", "IsActive", "Operator", "RuleId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 10, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BindTypeCode", true, "Equals", 4, null, "PB" },
                    { 11, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IsCountry", true, "Equals", 4, null, "US" },
                    { 12, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PrintQuantity", true, "LessThanOrEqual", 4, null, "20" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "tbl_RuleCondition",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "tbl_RuleCondition",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "tbl_RuleCondition",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "tbl_Rule",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
