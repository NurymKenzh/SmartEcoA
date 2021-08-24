using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarModelAutoTest_20210817_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModelAutoTest_TypeEcoClass_TypeEcoClassId",
                table: "CarModelAutoTest");

            migrationBuilder.AlterColumn<int>(
                name: "TypeEcoClassId",
                table: "CarModelAutoTest",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CarModelAutoTest_TypeEcoClass_TypeEcoClassId",
                table: "CarModelAutoTest",
                column: "TypeEcoClassId",
                principalTable: "TypeEcoClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarModelAutoTest_TypeEcoClass_TypeEcoClassId",
                table: "CarModelAutoTest");

            migrationBuilder.AlterColumn<int>(
                name: "TypeEcoClassId",
                table: "CarModelAutoTest",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarModelAutoTest_TypeEcoClass_TypeEcoClassId",
                table: "CarModelAutoTest",
                column: "TypeEcoClassId",
                principalTable: "TypeEcoClass",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
