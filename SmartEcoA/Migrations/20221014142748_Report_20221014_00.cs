using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class Report_20221014_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDF",
                table: "Report");

            migrationBuilder.AddColumn<int>(
                name: "TypeReport",
                table: "Report",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TypeReport",
                table: "Report");

            migrationBuilder.AddColumn<bool>(
                name: "PDF",
                table: "Report",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
