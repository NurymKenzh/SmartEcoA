using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class Report_20210803_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CarPostEndDate",
                table: "Report",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CarPostStartDate",
                table: "Report",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarPostEndDate",
                table: "Report");

            migrationBuilder.DropColumn(
                name: "CarPostStartDate",
                table: "Report");
        }
    }
}
