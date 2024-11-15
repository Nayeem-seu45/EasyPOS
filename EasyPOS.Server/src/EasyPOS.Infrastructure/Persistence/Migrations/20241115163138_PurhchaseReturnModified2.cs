using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PurhchaseReturnModified2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TaxRate",
                table: "PurchaseReturns",
                type: "decimal(4,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxAmount",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ShippingCost",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountRate",
                table: "PurchaseReturns",
                type: "decimal(3,2)",
                nullable: false,
                defaultValue: 1m,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TaxRate",
                table: "PurchaseReturns",
                type: "decimal(4,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(4,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "TaxAmount",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "ShippingCost",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountRate",
                table: "PurchaseReturns",
                type: "decimal(3,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(3,2)",
                oldDefaultValue: 1m);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountAmount",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);
        }
    }
}
