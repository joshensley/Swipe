using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class AddAboutModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AboutID",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "About",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AboutMe = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_About", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AboutID",
                table: "AspNetUsers",
                column: "AboutID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_About_AboutID",
                table: "AspNetUsers",
                column: "AboutID",
                principalTable: "About",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_About_AboutID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "About");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AboutID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AboutID",
                table: "AspNetUsers");
        }
    }
}
