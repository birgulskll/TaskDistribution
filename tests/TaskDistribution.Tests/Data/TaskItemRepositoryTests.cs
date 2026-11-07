using TaskDistribution.Data.Repositories;

namespace TaskDistribution.Tests.Data;

public class TaskItemRepositoryTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    [Fact]
    public async Task EkleAsync_GoreviKaydeder()
    {
        // Arrange
        var gorev = TestVerisi.Gorev(personelId: 4, islemId: 8, zorluk: 5);

        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            await new TaskItemRepository(context).EkleAsync(gorev);
        }

        // Assert
        using (var context = _veritabani.ContextOlustur())
        {
            var gorevler = await new TaskItemRepository(context).TumunuGetirAsync();
            var kaydedilen = Assert.Single(gorevler);
            Assert.Equal(gorev.Id, kaydedilen.Id);
            Assert.Equal(5, kaydedilen.Zorluk);
        }
    }

    [Fact]
    public async Task TumunuGetirAsync_EnYeniGoreviEnUsteKoyarVeIliskileriYukler()
    {
        // Arrange: farklı tarihlerde iki görev
        using (var context = _veritabani.ContextOlustur())
        {
            context.Gorevler.AddRange(
                TestVerisi.Gorev(personelId: 1, islemId: 1, zorluk: 1, tarih: new DateTime(2026, 11, 1)),
                TestVerisi.Gorev(personelId: 2, islemId: 3, zorluk: 2, tarih: new DateTime(2026, 11, 5)));
            await context.SaveChangesAsync();
        }

        // Act
        using (var context = _veritabani.ContextOlustur())
        {
            var gorevler = await new TaskItemRepository(context).TumunuGetirAsync();

            // Assert
            Assert.Equal(2, gorevler.Count);
            Assert.Equal(new DateTime(2026, 11, 5), gorevler[0].OlusturmaTarihi);   // En yeni üstte
            Assert.Equal("Arayüz Düzenleme", gorevler[0].Islem.Ad);                 // Include çalıştı
            Assert.Equal("Mehmet Kaya", gorevler[0].AtananPersonel.TamAd);
        }
    }
}
