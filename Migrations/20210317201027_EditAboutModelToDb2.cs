using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class EditAboutModelToDb2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AboutMe",
                table: "About",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ICouldProbablyBeatYouAt",
                table: "About",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IValue",
                table: "About",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastShowBingeWatched",
                table: "About",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatImLookingFor",
                table: "About",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ICouldProbablyBeatYouAt",
                table: "About");

            migrationBuilder.DropColumn(
                name: "IValue",
                table: "About");

            migrationBuilder.DropColumn(
                name: "LastShowBingeWatched",
                table: "About");

            migrationBuilder.DropColumn(
                name: "WhatImLookingFor",
                table: "About");

            migrationBuilder.AlterColumn<string>(
                name: "AboutMe",
                table: "About",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
