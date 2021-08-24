using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarModelAutoTest_20210817_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CarModelAutoTest",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeEcoClassId",
                table: "CarModelAutoTest",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CarModelAutoTest_TypeEcoClassId",
                table: "CarModelAutoTest",
                column: "TypeEcoClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarModelAutoTest_TypeEcoClass_TypeEcoClassId",
                table: "CarModelAutoTest",
                column: "TypeEcoClassId",
                principalTable: "TypeEcoClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModelAutoTest_TypeEcoClass_TypeEcoClassId",
                table: "CarModelAutoTest");

            migrationBuilder.DropIndex(
                name: "IX_CarModelAutoTest_TypeEcoClassId",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CarModelAutoTest");

            migrationBuilder.DropColumn(
                name: "TypeEcoClassId",
                table: "CarModelAutoTest");

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_CO2",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_NOx",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MAX_O2",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_CO2",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_NOx",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MIN_O2",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "CarModelAutoTest",
                type: "integer",
                nullable: true);
        }
    }
}
