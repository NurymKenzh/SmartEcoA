using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SmartEcoA.Migrations
{
    public partial class PostDataAvg_20210130_00 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostDataAvg",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateTime = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    MeasuredParameterId = table.Column<int>(nullable: false),
                    PostId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostDataAvg", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostDataAvg_MeasuredParameter_MeasuredParameterId",
                        column: x => x.MeasuredParameterId,
                        principalTable: "MeasuredParameter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostDataAvg_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostDataAvg_MeasuredParameterId",
                table: "PostDataAvg",
                column: "MeasuredParameterId");

            migrationBuilder.CreateIndex(
                name: "IX_PostDataAvg_PostId",
                table: "PostDataAvg",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostDataAvg");
        }
    }
}
