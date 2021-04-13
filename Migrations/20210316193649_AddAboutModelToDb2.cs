using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class AddAboutModelToDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_About_AboutID",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<Guid>(
                name: "ApplicationUserID",
                table: "About",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_About_AboutID",
                table: "AspNetUsers",
                column: "AboutID",
                principalTable: "About",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_About_AboutID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ApplicationUserID",
                table: "About");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_About_AboutID",
                table: "AspNetUsers",
                column: "AboutID",
                principalTable: "About",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
