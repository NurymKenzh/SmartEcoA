using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class CarPostDataSmokeMeter_20210208_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarPostDataSmokeMeter",
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
                    CarModelSmokeMeterId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPostDataSmokeMeter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarPostDataSmokeMeter_CarModelSmokeMeter_CarModelSmokeMeter~",
                        column: x => x.CarModelSmokeMeterId,
                        principalTable: "CarModelSmokeMeter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarPostDataSmokeMeter_CarModelSmokeMeterId",
                table: "CarPostDataSmokeMeter",
                column: "CarModelSmokeMeterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPostDataSmokeMeter");
        }
    }
}
