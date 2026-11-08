using Microsoft.Extensions.Logging.Abstractions;
using TaskDistribution.Business.Services;
using TaskDistribution.Data;
using TaskDistribution.Data.Repositories;
using TaskDistribution.Tests.Data;

namespace TaskDistribution.Tests.Business;

/// <summary>
/// GorevService'i gerçek repository'ler, gerçek atama algoritması ve
/// bellek içi veritabanıyla birlikte test eder (uçtan uca iş akışı).
/// </summary>
public class GorevServiceTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    // Seed data hatırlatma: işlem 1 → zorluk 1, işlem 5 → zorluk 3, işlem 8 → zorluk 5
    private const int Zorluk1Islem = 1;
    private const int Zorluk3Islem = 5;
    private const int Zorluk5Islem = 8;

    private static GorevService ServisOlustur(AppDbContext context)
    {
        return new GorevService(
            new TaskItemRepository(context),
            new IslemRepository(context),
            new PersonelRepository(context),
            new AtamaService(),
            NullLogger<GorevService>.Instance);
    }

    [Fact]
    public async Task OlusturAsync_ZorluguSecilenIslemdenAlirVeKaydeder()
    {
        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            var sonuc = await ServisOlustur(context).OlusturAsync("Sistem mimarisi", "  ", Zorluk5Islem);

            Assert.True(sonuc.Basarili);
            Assert.False(sonuc.UyariVar);
        }

        // Assert
        using (var context = _veritabani.ContextOlustur())
        {
            var gorev = context.Gorevler.Single();
            Assert.Equal(5, gorev.Zorluk);              // İşlem 8'in zorluğu
            Assert.Equal(Zorluk5Islem, gorev.IslemId);
            Assert.Equal(1, gorev.AtananPersonelId);    // Herkes eşit → Id'si en küçük olan
            Assert.Null(gorev.Aciklama);                // Sadece boşluktan oluşan açıklama null olur
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public async Task OlusturAsync_BosBaslik_HataDonerVeKaydetmez(string? baslik)
    {
        using var context = _veritabani.ContextOlustur();

        var sonuc = await ServisOlustur(context).OlusturAsync(baslik, null, Zorluk1Islem);

        Assert.False(sonuc.Basarili);
        Assert.Equal("Görev başlığı boş olamaz.", sonuc.Mesaj);
        Assert.Empty(context.Gorevler);
    }

    [Fact]
    public async Task OlusturAsync_OlmayanIslem_HataDonerVeKaydetmez()
    {
        using var context = _veritabani.ContextOlustur();

        var sonuc = await ServisOlustur(context).OlusturAsync("Bir görev", null, islemId: 999);

        Assert.False(sonuc.Basarili);
        Assert.Equal("Seçilen işlem bulunamadı.", sonuc.Mesaj);
        Assert.Empty(context.Gorevler);
    }

    [Fact]
    public async Task OlusturAsync_AktifPersonelYoksa_AnlamliHataDonerVeKaydetmez()
    {
        // Arrange: tüm personelleri pasif yap
        using (var context = _veritabani.ContextOlustur())
        {
            foreach (var personel in context.Personeller)
            {
                personel.AktifMi = false;
            }
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            var sonuc = await ServisOlustur(context).OlusturAsync("Bir görev", null, Zorluk1Islem);

            // Assert
            Assert.False(sonuc.Basarili);
            Assert.Contains("aktif personel bulunmuyor", sonuc.Mesaj);
            Assert.Empty(context.Gorevler);
        }
    }

    [Fact]
    public async Task OlusturAsync_KuralEsnetilirse_GorevKaydedilirAmaUyariDoner()
    {
        // Arrange: sadece 1 numaralı personel aktif kalsın
        using (var context = _veritabani.ContextOlustur())
        {
            foreach (var personel in context.Personeller.Where(p => p.Id != 1))
            {
                personel.AktifMi = false;
            }
            await context.SaveChangesAsync();
        }

        // Act: aynı zorlukta art arda iki görev
        using (var context = _veritabani.ContextOlustur())
        {
            var servis = ServisOlustur(context);
            var birinci = await servis.OlusturAsync("Birinci", null, Zorluk3Islem);
            var ikinci = await servis.OlusturAsync("İkinci", null, Zorluk3Islem);

            // Assert
            Assert.False(birinci.UyariVar);
            Assert.True(ikinci.Basarili);
            Assert.True(ikinci.UyariVar);   // Başka seçenek olmadığı için kural esnetildi
            Assert.Equal(2, context.Gorevler.Count(g => g.AtananPersonelId == 1));
        }
    }

    [Fact]
    public async Task AyniZorlukta8Gorev_8FarkliPersoneleAdilceDagitilir()
    {
        // Act: her görev ayrı bir "web isteği" gibi, yeni bir context ile oluşturuluyor
        for (int i = 1; i <= 8; i++)
        {
            using var context = _veritabani.ContextOlustur();
            await ServisOlustur(context).OlusturAsync($"Rapor {i}", null, Zorluk3Islem);
        }

        // Assert: 8 personelin her biri tam olarak 1 görev almış olmalı
        using (var kontrol = _veritabani.ContextOlustur())
        {
            var personelBasinaGorev = kontrol.Gorevler
                .GroupBy(g => g.AtananPersonelId)
                .Select(grup => grup.Count())
                .ToList();

            Assert.Equal(8, personelBasinaGorev.Count);              // 8 farklı personel
            Assert.All(personelBasinaGorev, adet => Assert.Equal(1, adet));
        }
    }
}
