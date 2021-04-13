using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class EditPreferencesModelInDb3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GenderID",
                table: "Preferences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "HeightID",
                table: "Preferences",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "PreferencesID",
                table: "Heights",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PreferencesID",
                table: "Gender",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Heights_PreferencesID",
                table: "Heights",
                column: "PreferencesID");

            migrationBuilder.CreateIndex(
                name: "IX_Gender_PreferencesID",
                table: "Gender",
                column: "PreferencesID");

            migrationBuilder.AddForeignKey(
                name: "FK_Gender_Preferences_PreferencesID",
                table: "Gender",
                column: "PreferencesID",
                principalTable: "Preferences",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Heights_Preferences_PreferencesID",
                table: "Heights",
                column: "PreferencesID",
                principalTable: "Preferences",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gender_Preferences_PreferencesID",
                table: "Gender");

            migrationBuilder.DropForeignKey(
                name: "FK_Heights_Preferences_PreferencesID",
                table: "Heights");

            migrationBuilder.DropIndex(
                name: "IX_Heights_PreferencesID",
                table: "Heights");

            migrationBuilder.DropIndex(
                name: "IX_Gender_PreferencesID",
                table: "Gender");

            migrationBuilder.DropColumn(
                name: "GenderID",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "HeightID",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "PreferencesID",
                table: "Heights");

            migrationBuilder.DropColumn(
                name: "PreferencesID",
                table: "Gender");
        }
    }
}
