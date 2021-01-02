using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class MeasuredParameter_20201230_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MeasuredParameter",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameKK = table.Column<string>(nullable: true),
                    NameRU = table.Column<string>(nullable: true),
                    NameEN = table.Column<string>(nullable: true),
                    MPCDailyAverage = table.Column<decimal>(nullable: true),
                    MPCMaxOneTime = table.Column<decimal>(nullable: true),
                    OceanusCode = table.Column<string>(nullable: true),
                    KazhydrometCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredParameter", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasuredParameter");
        }
    }
}
