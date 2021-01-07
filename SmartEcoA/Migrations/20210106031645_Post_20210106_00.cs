using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class Post_20210106_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Post",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true),
                    MN = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(nullable: false),
                    Longitude = table.Column<decimal>(nullable: false),
                    Information = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    ProjectId = table.Column<int>(nullable: true),
                    PollutionEnvironmentId = table.Column<int>(nullable: false),
                    DataProviderId = table.Column<int>(nullable: false),
                    KazhydrometID = table.Column<int>(nullable: true),
                    Automatic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Post", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Post_DataProvider_DataProviderId",
                        column: x => x.DataProviderId,
                        principalTable: "DataProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_PollutionEnvironment_PollutionEnvironmentId",
                        column: x => x.PollutionEnvironmentId,
                        principalTable: "PollutionEnvironment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Post_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Post_DataProviderId",
                table: "Post",
                column: "DataProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_PollutionEnvironmentId",
                table: "Post",
                column: "PollutionEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Post_ProjectId",
                table: "Post",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Post");
        }
    }
}
