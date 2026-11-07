using TaskDistribution.Business.Services;
using TaskDistribution.Data;
using TaskDistribution.Data.Repositories;
using TaskDistribution.Tests.Data;

namespace TaskDistribution.Tests.Business;

public class OzetServiceTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    private static OzetService ServisOlustur(AppDbContext context)
    {
        return new OzetService(new PersonelRepository(context));
    }

    [Fact]
    public async Task HicGorevYokken_TumPersonellerSifirGorevleListelenir()
    {
        using var context = _veritabani.ContextOlustur();

        var ozetler = await ServisOlustur(context).PersonelOzetleriniGetirAsync();

        Assert.Equal(8, ozetler.Count);
        Assert.All(ozetler, o =>
        {
            Assert.Equal(0, o.ToplamGorev);
            Assert.Null(o.SonGorevZorlugu);
            Assert.Equal(new[] { 1, 2, 3, 4, 5 }, o.ZorlukSayilari.Keys.OrderBy(k => k));   // 5 sütun her zaman var
        });
    }

    [Fact]
    public async Task GorevleriZorlugaGoreSayarVeSonGoreviBulur()
    {
        // Arrange: 1 numaralı personele (Ayşe) 3 görev ver
        using (var context = _veritabani.ContextOlustur())
        {
            context.Gorevler.AddRange(
                TestVerisi.Gorev(personelId: 1, islemId: 1, zorluk: 1, tarih: new DateTime(2026, 11, 1)),
                TestVerisi.Gorev(personelId: 1, islemId: 2, zorluk: 1, tarih: new DateTime(2026, 11, 2)),
                TestVerisi.Gorev(personelId: 1, islemId: 8, zorluk: 5, tarih: new DateTime(2026, 11, 3)));
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            var ayse = (await ServisOlustur(context).PersonelOzetleriniGetirAsync())
                .Single(o => o.PersonelId == 1);

            // Assert
            Assert.Equal(3, ayse.ToplamGorev);
            Assert.Equal(2, ayse.ZorlukSayilari[1]);
            Assert.Equal(0, ayse.ZorlukSayilari[3]);
            Assert.Equal(1, ayse.ZorlukSayilari[5]);
            Assert.Equal(5, ayse.SonGorevZorlugu);
            Assert.Equal(new DateTime(2026, 11, 3), ayse.SonAtanmaTarihi);
        }
    }

    [Fact]
    public async Task PasifPersonelDeOzeteDahilEdilir()
    {
        using (var context = _veritabani.ContextOlustur())
        {
            context.Personeller.Find(2)!.AktifMi = false;
            await context.SaveChangesAsync();
        }

        using (var context = _veritabani.ContextOlustur())
        {
            var ozetler = await ServisOlustur(context).PersonelOzetleriniGetirAsync();

            Assert.Equal(8, ozetler.Count);
            Assert.False(ozetler.Single(o => o.PersonelId == 2).AktifMi);
        }
    }
}
