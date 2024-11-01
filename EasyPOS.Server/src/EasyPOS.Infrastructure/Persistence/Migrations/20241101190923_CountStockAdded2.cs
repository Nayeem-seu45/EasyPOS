using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CountStockAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CountStockCategories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CountStockBrands",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_CountStockCategories_CountStockId",
                table: "CountStockCategories",
                column: "CountStockId");

            migrationBuilder.CreateIndex(
                name: "IX_CountStockBrands_CountStockId",
                table: "CountStockBrands",
                column: "CountStockId");

            migrationBuilder.AddForeignKey(
                name: "FK_CountStockBrands_CountStocks_CountStockId",
                table: "CountStockBrands",
                column: "CountStockId",
                principalTable: "CountStocks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CountStockCategories_CountStocks_CountStockId",
                table: "CountStockCategories",
                column: "CountStockId",
                principalTable: "CountStocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CountStockBrands_CountStocks_CountStockId",
                table: "CountStockBrands");

            migrationBuilder.DropForeignKey(
                name: "FK_CountStockCategories_CountStocks_CountStockId",
                table: "CountStockCategories");

            migrationBuilder.DropIndex(
                name: "IX_CountStockCategories_CountStockId",
                table: "CountStockCategories");

            migrationBuilder.DropIndex(
                name: "IX_CountStockBrands_CountStockId",
                table: "CountStockBrands");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CountStockCategories",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CountStockBrands",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");
        }
    }
}
