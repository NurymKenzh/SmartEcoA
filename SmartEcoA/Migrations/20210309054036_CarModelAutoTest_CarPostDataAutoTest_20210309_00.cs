using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class CarModelAutoTest_CarPostDataAutoTest_20210309_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarModelAutoTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    EngineType = table.Column<int>(nullable: false),
                    MIN_TAH = table.Column<decimal>(nullable: false),
                    DEL_MIN = table.Column<decimal>(nullable: false),
                    MAX_TAH = table.Column<decimal>(nullable: false),
                    DEL_MAX = table.Column<decimal>(nullable: false),
                    MIN_CO = table.Column<decimal>(nullable: false),
                    MAX_CO = table.Column<decimal>(nullable: false),
                    MIN_CH = table.Column<decimal>(nullable: false),
                    MAX_CH = table.Column<decimal>(nullable: false),
                    L_MIN = table.Column<decimal>(nullable: false),
                    L_MAX = table.Column<decimal>(nullable: false),
                    K_SVOB = table.Column<decimal>(nullable: false),
                    K_MAX = table.Column<decimal>(nullable: false),
                    CarPostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModelAutoTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarModelAutoTest_CarPost_CarPostId",
                        column: x => x.CarPostId,
                        principalTable: "CarPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CarPostDataAutoTest",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Number = table.Column<string>(nullable: true),
                    DOPOL1 = table.Column<string>(nullable: true),
                    DOPOL2 = table.Column<string>(nullable: true),
                    MIN_TAH = table.Column<decimal>(nullable: false),
                    MIN_CO = table.Column<decimal>(nullable: false),
                    MIN_CH = table.Column<decimal>(nullable: false),
                    MIN_CO2 = table.Column<decimal>(nullable: false),
                    MIN_O2 = table.Column<decimal>(nullable: false),
                    MIN_L = table.Column<decimal>(nullable: false),
                    MAX_TAH = table.Column<decimal>(nullable: false),
                    MAX_CO = table.Column<decimal>(nullable: false),
                    MAX_CH = table.Column<decimal>(nullable: false),
                    MAX_CO2 = table.Column<decimal>(nullable: false),
                    MAX_O2 = table.Column<decimal>(nullable: false),
                    MAX_L = table.Column<decimal>(nullable: false),
                    ZAV_NOMER = table.Column<decimal>(nullable: false),
                    K_1 = table.Column<decimal>(nullable: false),
                    K_2 = table.Column<decimal>(nullable: false),
                    K_3 = table.Column<decimal>(nullable: false),
                    K_4 = table.Column<decimal>(nullable: false),
                    K_SVOB = table.Column<decimal>(nullable: false),
                    K_MAX = table.Column<decimal>(nullable: false),
                    MIN_NO = table.Column<decimal>(nullable: false),
                    MAX_NO = table.Column<decimal>(nullable: false),
                    CarModelAutoTestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPostDataAutoTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarPostDataAutoTest_CarModelAutoTest_CarModelAutoTestId",
                        column: x => x.CarModelAutoTestId,
                        principalTable: "CarModelAutoTest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarModelAutoTest_CarPostId",
                table: "CarModelAutoTest",
                column: "CarPostId");

            migrationBuilder.CreateIndex(
                name: "IX_CarPostDataAutoTest_CarModelAutoTestId",
                table: "CarPostDataAutoTest",
                column: "CarModelAutoTestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPostDataAutoTest");

            migrationBuilder.DropTable(
                name: "CarModelAutoTest");
        }
    }
}
