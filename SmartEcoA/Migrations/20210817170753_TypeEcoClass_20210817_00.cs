using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class TypeEcoClass_20210817_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TypeEcoClass",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    EngineType = table.Column<decimal>(nullable: true),
                    MIN_TAH = table.Column<decimal>(nullable: true),
                    DEL_MIN = table.Column<decimal>(nullable: true),
                    MAX_TAH = table.Column<decimal>(nullable: true),
                    DEL_MAX = table.Column<decimal>(nullable: true),
                    MIN_CO = table.Column<decimal>(nullable: true),
                    MAX_CO = table.Column<decimal>(nullable: true),
                    MIN_CH = table.Column<decimal>(nullable: true),
                    MAX_CH = table.Column<decimal>(nullable: true),
                    L_MIN = table.Column<decimal>(nullable: true),
                    L_MAX = table.Column<decimal>(nullable: true),
                    K_SVOB = table.Column<decimal>(nullable: true),
                    K_MAX = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeEcoClass", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TypeEcoClass");
        }
    }
}
