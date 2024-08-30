using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportBot.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReportTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeOfShift",
                table: "Report");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TimeOfShift",
                table: "Report",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
