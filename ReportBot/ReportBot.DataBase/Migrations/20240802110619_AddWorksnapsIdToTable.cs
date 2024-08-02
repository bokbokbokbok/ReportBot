﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportBot.DataBase.Migrations
{
    /// <inheritdoc />
    public partial class AddWorksnapsIdToTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorksnapsId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorksnapsId",
                table: "Projects");
        }
    }
}
