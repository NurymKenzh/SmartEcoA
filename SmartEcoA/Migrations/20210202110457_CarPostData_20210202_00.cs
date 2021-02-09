using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class CarPostData_20210202_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarPostData",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    RunIn = table.Column<bool>(nullable: false),
                    DFree = table.Column<decimal>(nullable: false),
                    DMax = table.Column<decimal>(nullable: false),
                    NDFree = table.Column<decimal>(nullable: false),
                    NDMax = table.Column<decimal>(nullable: false),
                    CarModelId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPostData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarPostData_CarModel_CarModelId",
                        column: x => x.CarModelId,
                        principalTable: "CarModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarPostData_CarModelId",
                table: "CarPostData",
                column: "CarModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPostData");
        }
    }
}
