using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class AddPassModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pass",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationUserLikedID = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pass", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pass");
        }
    }
}
