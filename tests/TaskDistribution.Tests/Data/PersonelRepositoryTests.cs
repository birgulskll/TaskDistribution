using TaskDistribution.Data.Repositories;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Data;

public class PersonelRepositoryTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    [Fact]
    public async Task TumunuGetirAsync_SeedPersonelleriniAdaGoreSiraliDondurur()
    {
        // Arrange
        using var context = _veritabani.ContextOlustur();
        var repository = new PersonelRepository(context);

        // Act
        var personeller = await repository.TumunuGetirAsync();

        // Assert
        Assert.Equal(8, personeller.Count);
        Assert.Equal("Ali", personeller.First().Ad);
        Assert.Equal("Zeynep", personeller.Last().Ad);
    }

    [Fact]
    public async Task GetirAsync_VarOlanId_PersoneliDondurur()
    {
        using var context = _veritabani.ContextOlustur();
        var repository = new PersonelRepository(context);

        var personel = await repository.GetirAsync(1);

        Assert.NotNull(personel);
        Assert.Equal("Ayşe Yılmaz", personel.TamAd);
    }

    [Fact]
    public async Task GetirAsync_OlmayanId_NullDondurur()
    {
        using var context = _veritabani.ContextOlustur();
        var repository = new PersonelRepository(context);

        var personel = await repository.GetirAsync(999);

        Assert.Null(personel);
    }

    [Fact]
    public async Task EkleAsync_PersoneliKaydederVeIdAtar()
    {
        // Arrange
        var yeniPersonel = new Personel { Ad = "Deniz", Soyad = "Ak" };

        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            await new PersonelRepository(context).EkleAsync(yeniPersonel);
        }

        // Assert: veritabanı yeni bir Id vermiş olmalı ve kayıt okunabilmeli
        Assert.Equal(9, yeniPersonel.Id);

        using (var context = _veritabani.ContextOlustur())
        {
            var okunan = await new PersonelRepository(context).GetirAsync(yeniPersonel.Id);
            Assert.NotNull(okunan);
            Assert.Equal("Deniz Ak", okunan.TamAd);
        }
    }

    [Fact]
    public async Task GuncelleAsync_PersoneliPasifYapar()
    {
        // Arrange & Act: 3 numaralı personeli pasif yap
        using (var context = _veritabani.ContextOlustur())
        {
            var repository = new PersonelRepository(context);
            var personel = (await repository.GetirAsync(3))!;
            personel.AktifMi = false;
            await repository.GuncelleAsync(personel);
        }

        // Assert
        using (var context = _veritabani.ContextOlustur())
        {
            var personel = await new PersonelRepository(context).GetirAsync(3);
            Assert.False(personel!.AktifMi);
        }
    }

    [Fact]
    public async Task AktifleriGorevleriyleGetirAsync_PasifleriGetirmezVeGorevleriYukler()
    {
        // Arrange: 2 numaralı personeli pasif yap, 1 numaralıya iki görev ver
        using (var context = _veritabani.ContextOlustur())
        {
            context.Personeller.Find(2)!.AktifMi = false;
            context.Gorevler.AddRange(
                TestVerisi.Gorev(personelId: 1, islemId: 1, zorluk: 1),
                TestVerisi.Gorev(personelId: 1, islemId: 5, zorluk: 3));
            await context.SaveChangesAsync();
        }

        // Act
        List<Personel> aktifler;
        using (var context = _veritabani.ContextOlustur())
        {
            aktifler = await new PersonelRepository(context).AktifleriGorevleriyleGetirAsync();
        }

        // Assert
        Assert.Equal(7, aktifler.Count);
        Assert.DoesNotContain(aktifler, p => p.Id == 2);
        Assert.Equal(2, aktifler.Single(p => p.Id == 1).Gorevler.Count);
        Assert.Empty(aktifler.Single(p => p.Id == 3).Gorevler);
    }
}
