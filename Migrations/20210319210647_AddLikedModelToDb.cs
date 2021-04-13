using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class AddLikedModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Liked",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApplicationUserLikedID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Liked", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Liked_AspNetUsers_ApplicationUserID",
                        column: x => x.ApplicationUserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Liked_ApplicationUserID",
                table: "Liked",
                column: "ApplicationUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Liked");
        }
    }
}
