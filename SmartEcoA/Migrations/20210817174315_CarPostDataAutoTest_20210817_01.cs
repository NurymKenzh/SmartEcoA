using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class CarPostDataAutoTest_20210817_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TesterId",
                table: "CarPostDataAutoTest",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CarPostDataAutoTest_TesterId",
                table: "CarPostDataAutoTest",
                column: "TesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarPostDataAutoTest_Tester_TesterId",
                table: "CarPostDataAutoTest",
                column: "TesterId",
                principalTable: "Tester",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarPostDataAutoTest_Tester_TesterId",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropIndex(
                name: "IX_CarPostDataAutoTest_TesterId",
                table: "CarPostDataAutoTest");

            migrationBuilder.DropColumn(
                name: "TesterId",
                table: "CarPostDataAutoTest");
        }
    }
}
