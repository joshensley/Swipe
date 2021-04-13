using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class RemovedIsProfileImagePropertyInImageModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isProfileImage",
                table: "Images");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isProfileImage",
                table: "Images",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
