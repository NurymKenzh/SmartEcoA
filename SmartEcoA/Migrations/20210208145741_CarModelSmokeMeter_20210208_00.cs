using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class CarModelSmokeMeter_20210208_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CarModelSmokeMeter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    Boost = table.Column<bool>(nullable: false),
                    DFreeMark = table.Column<decimal>(nullable: true),
                    DMaxMark = table.Column<decimal>(nullable: true),
                    CarPostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarModelSmokeMeter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CarModelSmokeMeter_CarPost_CarPostId",
                        column: x => x.CarPostId,
                        principalTable: "CarPost",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarModelSmokeMeter_CarPostId",
                table: "CarModelSmokeMeter",
                column: "CarPostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CarModelSmokeMeter");
        }
    }
}
