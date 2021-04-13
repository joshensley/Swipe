using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class EditPreferencesModelinDb4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "PreferencesID",
                table: "Heights");

            migrationBuilder.DropColumn(
                name: "PreferencesID",
                table: "Gender");

            migrationBuilder.AddColumn<bool>(
                name: "SavedPreferences",
                table: "Preferences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Preferences_GenderID",
                table: "Preferences",
                column: "GenderID");

            migrationBuilder.CreateIndex(
                name: "IX_Preferences_HeightID",
                table: "Preferences",
                column: "HeightID");

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_Gender_GenderID",
                table: "Preferences",
                column: "GenderID",
                principalTable: "Gender",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Preferences_Heights_HeightID",
                table: "Preferences",
                column: "HeightID",
                principalTable: "Heights",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_Gender_GenderID",
                table: "Preferences");

            migrationBuilder.DropForeignKey(
                name: "FK_Preferences_Heights_HeightID",
                table: "Preferences");

            migrationBuilder.DropIndex(
                name: "IX_Preferences_GenderID",
                table: "Preferences");

            migrationBuilder.DropIndex(
                name: "IX_Preferences_HeightID",
                table: "Preferences");

            migrationBuilder.DropColumn(
                name: "SavedPreferences",
                table: "Preferences");

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
    }
}
