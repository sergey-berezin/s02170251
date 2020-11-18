using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WpfApp1.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlobPictures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Pixels = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlobPictures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlobPictures_Pictures_Id",
                        column: x => x.Id,
                        principalTable: "Pictures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlobPictures");
        }
    }
}
