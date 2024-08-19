using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportBot.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class DeleteShiftTime : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftTime",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShiftTime",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
