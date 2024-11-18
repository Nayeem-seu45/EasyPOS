using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyPOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class shiftchnged2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkingShiftDetails_WorkingShifts_WorkingShiftId",
                table: "WorkingShiftDetails");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "WorkingShiftDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "WorkingShiftDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeOnly),
                oldType: "time");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "WorkingShiftDetails",
                type: "uniqueidentifier",
                nullable: false,
                defaultValueSql: "NEWSEQUENTIALID()",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingShiftDetails_WorkingShifts_WorkingShiftId",
                table: "WorkingShiftDetails",
                column: "WorkingShiftId",
                principalTable: "WorkingShifts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkingShiftDetails_WorkingShifts_WorkingShiftId",
                table: "WorkingShiftDetails");

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "StartTime",
                table: "WorkingShiftDetails",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeOnly>(
                name: "EndTime",
                table: "WorkingShiftDetails",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0),
                oldClrType: typeof(TimeOnly),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "WorkingShiftDetails",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldDefaultValueSql: "NEWSEQUENTIALID()");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkingShiftDetails_WorkingShifts_WorkingShiftId",
                table: "WorkingShiftDetails",
                column: "WorkingShiftId",
                principalTable: "WorkingShifts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
