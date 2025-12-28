using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RulesetEvaluationSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_EvaluationLog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    InputJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Matched = table.Column<bool>(type: "bit", nullable: false),
                    MatchedRulesetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MatchedRuleName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProductionPlant = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EvaluatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_EvaluationLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Ruleset",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Ruleset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_Rule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulesetId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ProductionPlant = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_Rule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_Rule_tbl_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "tbl_Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_RulesetCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RulesetId = table.Column<int>(type: "int", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_RulesetCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_RulesetCondition_tbl_Ruleset_RulesetId",
                        column: x => x.RulesetId,
                        principalTable: "tbl_Ruleset",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_RuleCondition",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RuleId = table.Column<int>(type: "int", nullable: false),
                    Field = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_RuleCondition", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_RuleCondition_tbl_Rule_RuleId",
                        column: x => x.RuleId,
                        principalTable: "tbl_Rule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "tbl_Ruleset",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rules for Publisher 99990", true, "Ruleset One", null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rules for Publisher 99999", true, "Ruleset Two", null }
                });

            migrationBuilder.InsertData(
                table: "tbl_Rule",
                columns: new[] { "Id", "CreatedAt", "Description", "IsActive", "Name", "Priority", "ProductionPlant", "RulesetId", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PB books to US for Publisher 99990", true, "Rule 1", 1, "US", 1, null },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CV books to UK for Publisher 99999", true, "Rule 2", 1, "UK", 2, null },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PB books with high quantity to KGL for Publisher 99999", true, "Rule 3", 2, "KGL", 2, null }
                });

            migrationBuilder.InsertData(
                table: "tbl_RulesetCondition",
                columns: new[] { "Id", "CreatedAt", "Field", "IsActive", "Operator", "RulesetId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PublisherNumber", true, "Equals", 1, null, "99990" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrderMethod", true, "Equals", 1, null, "POD" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PublisherNumber", true, "Equals", 2, null, "99999" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "OrderMethod", true, "Equals", 2, null, "POD" }
                });

            migrationBuilder.InsertData(
                table: "tbl_RuleCondition",
                columns: new[] { "Id", "CreatedAt", "Field", "IsActive", "Operator", "RuleId", "UpdatedAt", "Value" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BindTypeCode", true, "Equals", 1, null, "PB" },
                    { 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IsCountry", true, "Equals", 1, null, "US" },
                    { 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PrintQuantity", true, "LessThanOrEqual", 1, null, "20" },
                    { 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BindTypeCode", true, "Equals", 2, null, "CV" },
                    { 5, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IsCountry", true, "Equals", 2, null, "UK" },
                    { 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PrintQuantity", true, "LessThanOrEqual", 2, null, "20" },
                    { 7, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "BindTypeCode", true, "Equals", 3, null, "PB" },
                    { 8, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "IsCountry", true, "Equals", 3, null, "US" },
                    { 9, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "PrintQuantity", true, "GreaterThanOrEqual", 3, null, "20" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_EvaluationLog_EvaluatedAt",
                table: "tbl_EvaluationLog",
                column: "EvaluatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_EvaluationLog_Matched_EvaluatedAt",
                table: "tbl_EvaluationLog",
                columns: new[] { "Matched", "EvaluatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_EvaluationLog_OrderId",
                table: "tbl_EvaluationLog",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Rule_Priority",
                table: "tbl_Rule",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Rule_RulesetId",
                table: "tbl_Rule",
                column: "RulesetId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_RuleCondition_RuleId",
                table: "tbl_RuleCondition",
                column: "RuleId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_Ruleset_Name",
                table: "tbl_Ruleset",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_RulesetCondition_RulesetId",
                table: "tbl_RulesetCondition",
                column: "RulesetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_EvaluationLog");

            migrationBuilder.DropTable(
                name: "tbl_RuleCondition");

            migrationBuilder.DropTable(
                name: "tbl_RulesetCondition");

            migrationBuilder.DropTable(
                name: "tbl_Rule");

            migrationBuilder.DropTable(
                name: "tbl_Ruleset");
        }
    }
}
