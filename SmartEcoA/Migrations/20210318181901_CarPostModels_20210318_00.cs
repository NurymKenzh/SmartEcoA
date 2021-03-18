using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarPostModels_20210318_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarPostDataAutoTest_CarModelAutoTest_CarModelAutoTestId",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropForeignKey(
                name: "FK_CarPostDataSmokeMeter_CarModelSmokeMeter_CarModelSmokeMeter~",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.AlterColumn<int>(
                name: "CarModelSmokeMeterId",
                table: "CarPostDataSmokeMeter",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "CarModelAutoTestId",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_CarPostDataAutoTest_CarModelAutoTest_CarModelAutoTestId",
                table: "CarPostDataAutoTest",
                column: "CarModelAutoTestId",
                principalTable: "CarModelAutoTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CarPostDataSmokeMeter_CarModelSmokeMeter_CarModelSmokeMeter~",
                table: "CarPostDataSmokeMeter",
                column: "CarModelSmokeMeterId",
                principalTable: "CarModelSmokeMeter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarPostDataAutoTest_CarModelAutoTest_CarModelAutoTestId",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropForeignKey(
                name: "FK_CarPostDataSmokeMeter_CarModelSmokeMeter_CarModelSmokeMeter~",
                table: "CarPostDataSmokeMeter");

            migrationBuilder.AlterColumn<int>(
                name: "CarModelSmokeMeterId",
                table: "CarPostDataSmokeMeter",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CarModelAutoTestId",
                table: "CarPostDataAutoTest",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CarPostDataAutoTest_CarModelAutoTest_CarModelAutoTestId",
                table: "CarPostDataAutoTest",
                column: "CarModelAutoTestId",
                principalTable: "CarModelAutoTest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CarPostDataSmokeMeter_CarModelSmokeMeter_CarModelSmokeMeter~",
                table: "CarPostDataSmokeMeter",
                column: "CarModelSmokeMeterId",
                principalTable: "CarModelSmokeMeter",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
