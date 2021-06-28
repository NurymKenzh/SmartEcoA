using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarModelAutoTest_20210624_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "EngineType",
                table: "CarModelAutoTest",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "EngineType",
                table: "CarModelAutoTest",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
