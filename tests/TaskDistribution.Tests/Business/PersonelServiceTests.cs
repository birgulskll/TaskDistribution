using TaskDistribution.Business.Services;
using TaskDistribution.Data;
using TaskDistribution.Data.Repositories;
using TaskDistribution.Tests.Data;

namespace TaskDistribution.Tests.Business;

public class PersonelServiceTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    private static PersonelService ServisOlustur(AppDbContext context)
    {
        return new PersonelService(new PersonelRepository(context));
    }

    [Fact]
    public async Task EkleAsync_GecerliBilgi_BosluklariTemizleyipKaydeder()
    {
        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            var sonuc = await ServisOlustur(context).EkleAsync("  Deniz ", " Ak  ");

            Assert.True(sonuc.Basarili);
            Assert.Equal("Deniz Ak eklendi.", sonuc.Mesaj);
        }

        // Assert: veritabanında boşluksuz kaydedilmiş olmalı
        using (var context = _veritabani.ContextOlustur())
        {
            var personel = context.Personeller.Single(p => p.Ad == "Deniz");
            Assert.Equal("Ak", personel.Soyad);
            Assert.True(personel.AktifMi);
        }
    }

    [Theory]
    [InlineData(null, "Ak", "Ad boş olamaz.")]
    [InlineData("   ", "Ak", "Ad boş olamaz.")]
    [InlineData("Deniz", "", "Soyad boş olamaz.")]
    public async Task EkleAsync_EksikBilgi_HataDonerVeKaydetmez(string? ad, string? soyad, string beklenenMesaj)
    {
        using var context = _veritabani.ContextOlustur();

        var sonuc = await ServisOlustur(context).EkleAsync(ad, soyad);

        Assert.False(sonuc.Basarili);
        Assert.Equal(beklenenMesaj, sonuc.Mesaj);
        Assert.Equal(8, context.Personeller.Count());   // Sadece seed personelleri var
    }

    [Fact]
    public async Task EkleAsync_CokUzunAd_HataDoner()
    {
        using var context = _veritabani.ContextOlustur();
        string uzunAd = new string('a', 51);

        var sonuc = await ServisOlustur(context).EkleAsync(uzunAd, "Ak");

        Assert.False(sonuc.Basarili);
        Assert.Contains("en fazla 50 karakter", sonuc.Mesaj);
    }

    [Fact]
    public async Task GuncelleAsync_AdVeSoyadiDegistirir()
    {
        using (var context = _veritabani.ContextOlustur())
        {
            var sonuc = await ServisOlustur(context).GuncelleAsync(1, "Ayşegül", "Yıldız");
            Assert.True(sonuc.Basarili);
        }

        using (var context = _veritabani.ContextOlustur())
        {
            var personel = context.Personeller.Find(1)!;
            Assert.Equal("Ayşegül Yıldız", personel.TamAd);
        }
    }

    [Fact]
    public async Task GuncelleAsync_OlmayanPersonel_HataDoner()
    {
        using var context = _veritabani.ContextOlustur();

        var sonuc = await ServisOlustur(context).GuncelleAsync(999, "Ali", "Veli");

        Assert.False(sonuc.Basarili);
        Assert.Equal("Personel bulunamadı.", sonuc.Mesaj);
    }

    [Fact]
    public async Task AktiflikDurumunuDegistirAsync_IlkCagridaPasifIkincideAktifYapar()
    {
        using var context = _veritabani.ContextOlustur();
        var servis = ServisOlustur(context);

        var ilk = await servis.AktiflikDurumunuDegistirAsync(4);
        Assert.True(ilk.Basarili);
        Assert.Contains("pasif yapıldı", ilk.Mesaj);
        Assert.False(context.Personeller.Find(4)!.AktifMi);

        var ikinci = await servis.AktiflikDurumunuDegistirAsync(4);
        Assert.Contains("aktif yapıldı", ikinci.Mesaj);
        Assert.True(context.Personeller.Find(4)!.AktifMi);
    }

    [Fact]
    public async Task AktiflikDurumunuDegistirAsync_OlmayanPersonel_HataDoner()
    {
        using var context = _veritabani.ContextOlustur();

        var sonuc = await ServisOlustur(context).AktiflikDurumunuDegistirAsync(999);

        Assert.False(sonuc.Basarili);
    }

    // ---------- Silme (CRUD'un "D" harfi) ----------

    [Fact]
    public async Task SilAsync_GorevGecmisiYoksa_PersoneliSiler()
    {
        using (var context = _veritabani.ContextOlustur())
        {
            var sonuc = await ServisOlustur(context).SilAsync(8);

            Assert.True(sonuc.Basarili);
            Assert.Equal("Emre Aydın silindi.", sonuc.Mesaj);
        }

        using (var context = _veritabani.ContextOlustur())
        {
            Assert.Null(context.Personeller.Find(8));
        }
    }

    [Fact]
    public async Task SilAsync_GorevGecmisiVarsa_SilmezVePasifYapmayiOnerir()
    {
        // Arrange: 1 numaralı personele (Ayşe) bir görev ver
        using (var context = _veritabani.ContextOlustur())
        {
            context.Gorevler.Add(TestVerisi.Gorev(personelId: 1, islemId: 1, zorluk: 1));
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            var sonuc = await ServisOlustur(context).SilAsync(1);

            // Assert
            Assert.False(sonuc.Basarili);
            Assert.Contains("görev geçmişi olduğu için silinemez", sonuc.Mesaj);
            Assert.Contains("pasif", sonuc.Mesaj);
        }

        using (var context = _veritabani.ContextOlustur())
        {
            Assert.NotNull(context.Personeller.Find(1));   // Hâlâ yerinde
            Assert.Single(context.Gorevler);              // Görev geçmişi bozulmadı
        }
    }

    [Fact]
    public async Task SilAsync_OlmayanPersonel_HataDoner()
    {
        using var context = _veritabani.ContextOlustur();

        var sonuc = await ServisOlustur(context).SilAsync(999);

        Assert.False(sonuc.Basarili);
        Assert.Equal("Personel bulunamadı.", sonuc.Mesaj);
    }
}
