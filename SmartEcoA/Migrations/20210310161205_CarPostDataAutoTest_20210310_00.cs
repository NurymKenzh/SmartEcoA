using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarPostDataAutoTest_20210310_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ZAV_NOMER",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_TAH",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_O2",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_NO",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_L",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_CO2",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_CO",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_CH",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_TAH",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_O2",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_NO",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_L",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_CO2",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_CO",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_CH",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "K_SVOB",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "K_MAX",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "K_4",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "K_3",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "K_2",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<decimal>(
                name: "K_1",
                table: "CarPostDataAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "ZAV_NOMER",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_TAH",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_O2",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_NO",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_L",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_CO2",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_CO",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MIN_CH",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_TAH",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_O2",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_NO",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_L",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_CO2",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_CO",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MAX_CH",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "K_SVOB",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "K_MAX",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "K_4",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "K_3",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "K_2",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "K_1",
                table: "CarPostDataAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
