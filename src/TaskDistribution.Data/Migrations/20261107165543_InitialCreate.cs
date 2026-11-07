using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskDistribution.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Zorluk = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.Id);
                    table.CheckConstraint("CK_Islemler_Zorluk", "\"Zorluk\" BETWEEN 1 AND 5");
                });

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    AktifMi = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gorevler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Baslik = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    IslemId = table.Column<int>(type: "INTEGER", nullable: false),
                    AtananPersonelId = table.Column<int>(type: "INTEGER", nullable: false),
                    Zorluk = table.Column<int>(type: "INTEGER", nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gorevler", x => x.Id);
                    table.CheckConstraint("CK_Gorevler_Zorluk", "\"Zorluk\" BETWEEN 1 AND 5");
                    table.ForeignKey(
                        name: "FK_Gorevler_Islemler_IslemId",
                        column: x => x.IslemId,
                        principalTable: "Islemler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Gorevler_Personeller_AtananPersonelId",
                        column: x => x.AtananPersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Islemler",
                columns: new[] { "Id", "Ad", "Zorluk" },
                values: new object[,]
                {
                    { 1, "Doküman Güncelleme", 1 },
                    { 2, "Hata Kaydı İnceleme", 1 },
                    { 3, "Arayüz Düzenleme", 2 },
                    { 4, "Birim Test Yazma", 2 },
                    { 5, "Raporlama Ekranı", 3 },
                    { 6, "Veritabanı Optimizasyonu", 4 },
                    { 7, "Entegrasyon Geliştirme", 4 },
                    { 8, "Mimari Tasarım", 5 }
                });

            migrationBuilder.InsertData(
                table: "Personeller",
                columns: new[] { "Id", "Ad", "AktifMi", "Soyad" },
                values: new object[,]
                {
                    { 1, "Ayşe", true, "Yılmaz" },
                    { 2, "Mehmet", true, "Kaya" },
                    { 3, "Zeynep", true, "Demir" },
                    { 4, "Ali", true, "Çelik" },
                    { 5, "Elif", true, "Şahin" },
                    { 6, "Burak", true, "Arslan" },
                    { 7, "Selin", true, "Koç" },
                    { 8, "Emre", true, "Aydın" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gorevler_AtananPersonelId",
                table: "Gorevler",
                column: "AtananPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_Gorevler_IslemId",
                table: "Gorevler",
                column: "IslemId");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_Ad",
                table: "Islemler",
                column: "Ad",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Gorevler");

            migrationBuilder.DropTable(
                name: "Islemler");

            migrationBuilder.DropTable(
                name: "Personeller");
        }
    }
}
