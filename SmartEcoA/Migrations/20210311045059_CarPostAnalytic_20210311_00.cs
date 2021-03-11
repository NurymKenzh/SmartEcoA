using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class CarPostAnalytic_20210311_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarPostAnalytic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Measurement = table.Column<int>(nullable: false),
                    Exceeding = table.Column<int>(nullable: false),
                    CarPostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarPostAnalytic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarPostAnalytic_CarPost_CarPostId",
                        column: x => x.CarPostId,
                        principalTable: "CarPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarPostAnalytic_CarPostId",
                table: "CarPostAnalytic",
                column: "CarPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarPostAnalytic");
        }
    }
}
