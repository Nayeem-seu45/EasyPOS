using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class QuotationAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    QuotationDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WarehouseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    QuotationStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentStatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrandTotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuotationNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StaffNote = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    DueAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuotationDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWSEQUENTIALID()"),
                    QuotationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    ProductUnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductUnit = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ProductUnitDiscount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    BatchNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ExpiredDate = table.Column<DateOnly>(type: "date", nullable: true),
                    NetUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    DiscountRate = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TaxMethod = table.Column<int>(type: "int", nullable: false),
                    TaxRate = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    TaxAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuotationDetails_Quotations_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuotationDetails");

            migrationBuilder.DropTable(
                name: "Quotations");
        }
    }
}
