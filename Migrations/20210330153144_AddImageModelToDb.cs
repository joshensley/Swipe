using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class AddImageModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserIDId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ImageFirebaseURL = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Images_AspNetUsers_ApplicationUserIDId",
                        column: x => x.ApplicationUserIDId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_ApplicationUserIDId",
                table: "Images",
                column: "ApplicationUserIDId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Images");
        }
    }
}
