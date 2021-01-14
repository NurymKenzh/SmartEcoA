using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class PostDataDivided_20210114_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostDataDivided",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostDataId = table.Column<long>(nullable: false),
                    MN = table.Column<string>(nullable: true),
                    OceanusCode = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostDataDivided", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostDataDivided_PostData_PostDataId",
                        column: x => x.PostDataId,
                        principalTable: "PostData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostDataDivided_PostDataId",
                table: "PostDataDivided",
                column: "PostDataId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostDataDivided");
        }
    }
}
