using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarModelAutoTest_20210309_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "EngineType",
                table: "CarModelAutoTest",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "EngineType",
                table: "CarModelAutoTest",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
