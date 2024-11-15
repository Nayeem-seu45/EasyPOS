using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PurhchaseReturnModified : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "GrandTotal",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<decimal>(
                name: "DueAmount",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentStatusId",
                table: "PurchaseReturns",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueAmount",
                table: "PurchaseReturns");

            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "PurchaseReturns");

            migrationBuilder.DropColumn(
                name: "PaymentStatusId",
                table: "PurchaseReturns");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrandTotal",
                table: "PurchaseReturns",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);
        }
    }
}
