using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Swipe.Migrations
{
    public partial class AddEthnicityAndEthnicityPreferencesModelToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Preferences",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Preferences", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "EthnicityPreferences",
                columns: table => new
                {
                    EthnicityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreferencesID = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EthnicityPreferences", x => new { x.EthnicityID, x.PreferencesID });
                    table.ForeignKey(
                        name: "FK_EthnicityPreferences_Ethnicity_EthnicityID",
                        column: x => x.EthnicityID,
                        principalTable: "Ethnicity",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EthnicityPreferences_Preferences_PreferencesID",
                        column: x => x.PreferencesID,
                        principalTable: "Preferences",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EthnicityPreferences_PreferencesID",
                table: "EthnicityPreferences",
                column: "PreferencesID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EthnicityPreferences");

            migrationBuilder.DropTable(
                name: "Preferences");
        }
    }
}
