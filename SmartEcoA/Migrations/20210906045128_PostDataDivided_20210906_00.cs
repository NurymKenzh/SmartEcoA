using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartEcoA.Migrations
{
    public partial class PostDataDivided_20210906_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "PostDataAvgId",
                table: "PostDataDivided",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostDataDivided_PostDataAvgId",
                table: "PostDataDivided",
                column: "PostDataAvgId");

            migrationBuilder.AddForeignKey(
                name: "FK_PostDataDivided_PostDataAvg_PostDataAvgId",
                table: "PostDataDivided",
                column: "PostDataAvgId",
                principalTable: "PostDataAvg",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PostDataDivided_PostDataAvg_PostDataAvgId",
                table: "PostDataDivided");

            migrationBuilder.DropIndex(
                name: "IX_PostDataDivided_PostDataAvgId",
                table: "PostDataDivided");

            migrationBuilder.DropColumn(
                name: "PostDataAvgId",
                table: "PostDataDivided");
        }
    }
}
