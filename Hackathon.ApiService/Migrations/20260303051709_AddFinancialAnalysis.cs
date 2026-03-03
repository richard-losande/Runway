using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hackathon.ApiService.Migrations
{
    /// <inheritdoc />
    public partial class AddFinancialAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FinancialAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: true),
                    MonthlySalary = table.Column<decimal>(type: "numeric", nullable: false),
                    TotalSavings = table.Column<decimal>(type: "numeric", nullable: false),
                    RunwayMonths = table.Column<int>(type: "integer", nullable: false),
                    AdjustedRunwayMonths = table.Column<int>(type: "integer", nullable: false),
                    MonthlyBurnRate = table.Column<decimal>(type: "numeric", nullable: false),
                    ResponseJson = table.Column<string>(type: "jsonb", nullable: false),
                    AnalyzedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialAnalyses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FinancialAnalyses");
        }
    }
}
