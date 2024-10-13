using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reklamna_agencija.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminiAgencije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrTel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uloga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Profilna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatumRodjenja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminiAgencije", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Klijenti",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrTel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uloga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Profilna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatumRodjenja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klijenti", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Posetioci",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<bool>(type: "bit", nullable: true),
                    Prezime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrTel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uloga = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pol = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Profilna = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DatumRodjenja = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posetioci", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReklamniPanoi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminAgencijeId = table.Column<int>(type: "int", nullable: false),
                    UrlSlike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Adresa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Dimenzija = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Osvetljenost = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Grad = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Zona = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Cijena = table.Column<int>(type: "int", nullable: true),
                    StatusZauzetosti = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReklamniPanoi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReklamniPanoi_AdminiAgencije_AdminAgencijeId",
                        column: x => x.AdminAgencijeId,
                        principalTable: "AdminiAgencije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reklame",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KlijentId = table.Column<int>(type: "int", nullable: false),
                    ReklamniPanoId = table.Column<int>(type: "int", nullable: false),
                    UrlSlike = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OdDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DoDatum = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reklame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reklame_Klijenti_KlijentId",
                        column: x => x.KlijentId,
                        principalTable: "Klijenti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reklame_ReklamniPanoi_ReklamniPanoId",
                        column: x => x.ReklamniPanoId,
                        principalTable: "ReklamniPanoi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fakture",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminAgencijeId = table.Column<int>(type: "int", nullable: false),
                    ReklamaId = table.Column<int>(type: "int", nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Datum = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IznosNovcani = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fakture", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fakture_AdminiAgencije_AdminAgencijeId",
                        column: x => x.AdminAgencijeId,
                        principalTable: "AdminiAgencije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fakture_Reklame_ReklamaId",
                        column: x => x.ReklamaId,
                        principalTable: "Reklame",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fakture_AdminAgencijeId",
                table: "Fakture",
                column: "AdminAgencijeId");

            migrationBuilder.CreateIndex(
                name: "IX_Fakture_ReklamaId",
                table: "Fakture",
                column: "ReklamaId");

            migrationBuilder.CreateIndex(
                name: "IX_Reklame_KlijentId",
                table: "Reklame",
                column: "KlijentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reklame_ReklamniPanoId",
                table: "Reklame",
                column: "ReklamniPanoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReklamniPanoi_AdminAgencijeId",
                table: "ReklamniPanoi",
                column: "AdminAgencijeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fakture");

            migrationBuilder.DropTable(
                name: "Posetioci");

            migrationBuilder.DropTable(
                name: "Reklame");

            migrationBuilder.DropTable(
                name: "Klijenti");

            migrationBuilder.DropTable(
                name: "ReklamniPanoi");

            migrationBuilder.DropTable(
                name: "AdminiAgencije");
        }
    }
}
