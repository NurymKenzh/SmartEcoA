using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class CarPostData_20210209_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPostData");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarPostData",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CarModelId = table.Column<int>(type: "integer", nullable: false),
                    DFree = table.Column<decimal>(type: "numeric", nullable: false),
                    DMax = table.Column<decimal>(type: "numeric", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    NDFree = table.Column<decimal>(type: "numeric", nullable: false),
                    NDMax = table.Column<decimal>(type: "numeric", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: true),
                    RunIn = table.Column<bool>(type: "boolean", nullable: false)
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
    }
}
