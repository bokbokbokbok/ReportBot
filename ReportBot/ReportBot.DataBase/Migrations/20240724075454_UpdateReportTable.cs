using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportBot.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class UpdateReportTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "ChatId",
                table: "Report",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfShift",
                table: "Report",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TimeOfShift",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateOfShift",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "TimeOfShift",
                table: "Report");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "Report",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
