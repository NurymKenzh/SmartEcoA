using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarModelAutoTest_20210624_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CO2",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_NOx",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_O2",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CO2",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_NOx",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_O2",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CarModelAutoTest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MAX_CO2",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "MAX_NOx",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "MAX_O2",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "MIN_CO2",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "MIN_NOx",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "MIN_O2",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "CarModelAutoTest");
        }
    }
}
