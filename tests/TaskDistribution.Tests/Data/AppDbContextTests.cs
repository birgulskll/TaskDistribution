using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Data;

public class AppDbContextTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    [Fact]
    public void SeedData_8AktifPersonelEklenmisOlmali()
    {
        using var context = _veritabani.ContextOlustur();

        var personeller = context.Personeller.ToList();

        Assert.Equal(8, personeller.Count);
        Assert.All(personeller, p => Assert.True(p.AktifMi));
    }

    [Fact]
    public void SeedData_8IslemEklenmisVeHer1ile5ZorlukKullanilmisOlmali()
    {
        using var context = _veritabani.ContextOlustur();

        var islemler = context.Islemler.ToList();
        var kullanilanZorluklar = islemler.Select(i => i.Zorluk).Distinct().OrderBy(z => z);

        Assert.Equal(8, islemler.Count);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, kullanilanZorluklar);
    }

    [Fact]
    public void PasifPersonel_KaydedildigindePasifOlarakOkunmali()
    {
        // Arrange & Act: pasif bir personel kaydet
        using (var context = _veritabani.ContextOlustur())
        {
            context.Personeller.Add(new Personel { Ad = "Deniz", Soyad = "Ak", AktifMi = false });
            context.SaveChanges();
        }

        // Assert: yeni bir context ile veritabanından tekrar oku
        using (var context = _veritabani.ContextOlustur())
        {
            var personel = context.Personeller.Single(p => p.Ad == "Deniz");
            Assert.False(personel.AktifMi);
        }
    }

    [Fact]
    public void Gorev_KaydedildigindeIslemVePersoneliyleBirlikteOkunabilmeli()
    {
        // Arrange & Act
        using (var context = _veritabani.ContextOlustur())
        {
            context.Gorevler.Add(new TaskItem
            {
                Baslik = "Aylık rapor",
                IslemId = 5,            // Raporlama Ekranı (zorluk 3)
                AtananPersonelId = 2,   // Mehmet Kaya
                Zorluk = 3,
                OlusturmaTarihi = new DateTime(2026, 11, 7, 10, 0, 0)
            });
            context.SaveChanges();
        }

        // Assert: Include ile ilişkili tabloları da getir
        using (var context = _veritabani.ContextOlustur())
        {
            var gorev = context.Gorevler
                .Include(g => g.Islem)
                .Include(g => g.AtananPersonel)
                .Single();

            Assert.Equal("Raporlama Ekranı", gorev.Islem.Ad);
            Assert.Equal("Mehmet Kaya", gorev.AtananPersonel.TamAd);
            Assert.Null(gorev.Aciklama);
        }
    }

    [Fact]
    public void Veritabani_GecersizZorlukDegeriniReddetmeli()
    {
        using var context = _veritabani.ContextOlustur();

        // C# tarafındaki kontrolü atlayıp doğrudan SQL ile geçersiz değer yazmayı deniyoruz.
        // Check constraint bunu veritabanı seviyesinde engellemeli.
        var hata = Assert.Throws<SqliteException>(() =>
            context.Database.ExecuteSqlRaw(
                $"UPDATE Islemler SET Zorluk = {ZorlukSeviyesi.EnYuksek + 1} WHERE Id = 1"));

        Assert.Contains("CK_Islemler_Zorluk", hata.Message);
    }

    [Fact]
    public void GoreviOlanPersonel_Silinememeli()
    {
        // Arrange: 1 numaralı personele bir görev ver
        using (var context = _veritabani.ContextOlustur())
        {
            context.Gorevler.Add(new TaskItem
            {
                Baslik = "Doküman düzelt",
                IslemId = 1,
                AtananPersonelId = 1,
                Zorluk = 1,
                OlusturmaTarihi = new DateTime(2026, 11, 7)
            });
            context.SaveChanges();
        }

        // Act & Assert: personeli silmeye çalış (DeleteBehavior.Restrict buna izin vermemeli)
        using (var context = _veritabani.ContextOlustur())
        {
            var personel = context.Personeller.Find(1)!;
            context.Personeller.Remove(personel);

            Assert.Throws<DbUpdateException>(() => context.SaveChanges());
        }
    }
}
