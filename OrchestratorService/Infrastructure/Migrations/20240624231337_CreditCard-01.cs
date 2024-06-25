using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrchestratorService.Migrations
{
    /// <inheritdoc />
    public partial class CreditCard01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "CreditCards",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "CreditCards");
        }
    }
}
