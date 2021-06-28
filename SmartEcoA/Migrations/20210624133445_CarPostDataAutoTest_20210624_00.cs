using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarPostDataAutoTest_20210624_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ATNUM",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_NOx",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_NOx",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CarPostDataAutoTest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ATNUM",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "MAX_NOx",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "MIN_NOx",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CarPostDataAutoTest");
        }
    }
}
