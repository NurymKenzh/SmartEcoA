using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarPostDataAutoTest_20210817_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "GasCheckDate",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GasSerialNumber",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MeteoCheckDate",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MeteoSerialNumber",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pressure",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TestNumber",
                table: "CarPostDataAutoTest",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GasCheckDate",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "GasSerialNumber",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "MeteoCheckDate",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "MeteoSerialNumber",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "Pressure",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "TestNumber",
                table: "CarPostDataAutoTest");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "CarPostDataAutoTest",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ATNUM",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_NOx",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_NOx",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CarPostDataAutoTest",
                type: "integer",
                nullable: true);
        }
    }
}
