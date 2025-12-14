using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LernApp.Data.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    LernzeitProTag = table.Column<int>(type: "INTEGER", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AktualisiertAm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Prompts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    Response = table.Column<string>(type: "TEXT", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Kategorie = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prompts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Prompts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenerierteCSVs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    PromptId = table.Column<int>(type: "INTEGER", nullable: true),
                    Dateiname = table.Column<string>(type: "TEXT", nullable: false),
                    Inhalt = table.Column<string>(type: "TEXT", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Beschreibung = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenerierteCSVs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenerierteCSVs_Prompts_PromptId",
                        column: x => x.PromptId,
                        principalTable: "Prompts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GenerierteCSVs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LernEinheiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Fach = table.Column<string>(type: "TEXT", nullable: false),
                    Thema = table.Column<string>(type: "TEXT", nullable: false),
                    Beschreibung = table.Column<string>(type: "TEXT", nullable: true),
                    Datum = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    AktualisiertAm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LernEinheiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LernEinheiten_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DateiAnalysen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LernEinheitId = table.Column<int>(type: "INTEGER", nullable: false),
                    Dateiname = table.Column<string>(type: "TEXT", nullable: false),
                    InhaltZusammenfassung = table.Column<string>(type: "TEXT", nullable: false),
                    AnalysiertAm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DateityP = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateiAnalysen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateiAnalysen_LernEinheiten_LernEinheitId",
                        column: x => x.LernEinheitId,
                        principalTable: "LernEinheiten",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserEinstellungen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sprache = table.Column<string>(type: "TEXT", nullable: false),
                    Thema = table.Column<string>(type: "TEXT", nullable: false),
                    BenachrichtigungenAktiv = table.Column<bool>(type: "INTEGER", nullable: false),
                    AIModell = table.Column<string>(type: "TEXT", nullable: true),
                    AktualisiertAm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserEinstellungen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserEinstellungen_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prompts_UserId",
                table: "Prompts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerierteCSVs_PromptId",
                table: "GenerierteCSVs",
                column: "PromptId");

            migrationBuilder.CreateIndex(
                name: "IX_GenerierteCSVs_UserId",
                table: "GenerierteCSVs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LernEinheiten_UserId",
                table: "LernEinheiten",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DateiAnalysen_LernEinheitId",
                table: "DateiAnalysen",
                column: "LernEinheitId");

            migrationBuilder.CreateIndex(
                name: "IX_UserEinstellungen_UserId",
                table: "UserEinstellungen",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "DateiAnalysen");
            migrationBuilder.DropTable(name: "UserEinstellungen");
            migrationBuilder.DropTable(name: "GenerierteCSVs");
            migrationBuilder.DropTable(name: "LernEinheiten");
            migrationBuilder.DropTable(name: "Prompts");
            migrationBuilder.DropTable(name: "Users");
        }
    }
}
