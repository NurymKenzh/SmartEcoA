using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarModelSmokeMeter_CarPostDataSmokeMeter_20211001_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DFree",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DMax",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "NDFree",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "NDMax",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "RunIn",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "Boost",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DFreeMark",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DMaxMark",
                table: "CarModelSmokeMeter");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "CarPostDataSmokeMeter",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<string>(
                name: "DOPOL1",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DOPOL2",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GasCheckDate",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GasSerialNumber",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_1",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_2",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_3",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_4",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_MAX",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_SVOB",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CH",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CO",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CO2",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_L",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_NO",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_O2",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_TAH",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CH",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CO",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CO2",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_L",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_NO",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_O2",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_TAH",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MeteoCheckDate",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MeteoSerialNumber",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Pressure",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Temperature",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TestNumber",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TesterId",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ZAV_NOMER",
                table: "CarPostDataSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DEL_MAX",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DEL_MIN",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "EngineType",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_MAX",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "K_SVOB",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "L_MAX",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "L_MIN",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CH",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CO",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_TAH",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CH",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CO",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_TAH",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParadoxId",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeEcoClassId",
                table: "CarModelSmokeMeter",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarPostDataSmokeMeter_TesterId",
                table: "CarPostDataSmokeMeter",
                column: "TesterId");

            migrationBuilder.CreateIndex(
                name: "IX_CarModelSmokeMeter_TypeEcoClassId",
                table: "CarModelSmokeMeter",
                column: "TypeEcoClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarModelSmokeMeter_TypeEcoClass_TypeEcoClassId",
                table: "CarModelSmokeMeter",
                column: "TypeEcoClassId",
                principalTable: "TypeEcoClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarPostDataSmokeMeter_Tester_TesterId",
                table: "CarPostDataSmokeMeter",
                column: "TesterId",
                principalTable: "Tester",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModelSmokeMeter_TypeEcoClass_TypeEcoClassId",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropForeignKey(
                name: "FK_CarPostDataSmokeMeter_Tester_TesterId",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropIndex(
                name: "IX_CarPostDataSmokeMeter_TesterId",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropIndex(
                name: "IX_CarModelSmokeMeter_TypeEcoClassId",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DOPOL1",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DOPOL2",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "GasCheckDate",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "GasSerialNumber",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_1",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_2",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_3",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_4",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_MAX",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_SVOB",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_CH",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_CO",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_CO2",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_L",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_NO",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_O2",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_TAH",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_CH",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_CO",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_CO2",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_L",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_NO",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_O2",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_TAH",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MeteoCheckDate",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MeteoSerialNumber",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "Pressure",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "TestNumber",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "TesterId",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "ZAV_NOMER",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DEL_MAX",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "DEL_MIN",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "EngineType",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_MAX",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "K_SVOB",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "L_MAX",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "L_MIN",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_CH",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_CO",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MAX_TAH",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_CH",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_CO",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "MIN_TAH",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "ParadoxId",
                table: "CarModelSmokeMeter");

            migrationBuilder.DropColumn(
                name: "TypeEcoClassId",
                table: "CarModelSmokeMeter");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTime",
                table: "CarPostDataSmokeMeter",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DFree",
                table: "CarPostDataSmokeMeter",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DMax",
                table: "CarPostDataSmokeMeter",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NDFree",
                table: "CarPostDataSmokeMeter",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NDMax",
                table: "CarPostDataSmokeMeter",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RunIn",
                table: "CarPostDataSmokeMeter",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Boost",
                table: "CarModelSmokeMeter",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "DFreeMark",
                table: "CarModelSmokeMeter",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DMaxMark",
                table: "CarModelSmokeMeter",
                type: "numeric",
                nullable: true);
        }
    }
}
