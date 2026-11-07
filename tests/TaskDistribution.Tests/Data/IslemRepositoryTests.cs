using TaskDistribution.Data.Repositories;

namespace TaskDistribution.Tests.Data;

public class IslemRepositoryTests : IDisposable
{
    private readonly SqliteTestVeritabani _veritabani = new();

    public void Dispose() => _veritabani.Dispose();

    [Fact]
    public async Task TumunuGetirAsync_IslemleriZorlugaGoreSiraliDondurur()
    {
        using var context = _veritabani.ContextOlustur();
        var repository = new IslemRepository(context);

        var islemler = await repository.TumunuGetirAsync();

        Assert.Equal(8, islemler.Count);
        // Her işlemin zorluğu bir öncekinden küçük olmamalı (sıralı olmalı)
        Assert.Equal(islemler.Select(i => i.Zorluk).OrderBy(z => z), islemler.Select(i => i.Zorluk));
        Assert.Equal("Mimari Tasarım", islemler.Last().Ad);
    }

    [Fact]
    public async Task GetirAsync_VarOlanId_IslemiDondurur()
    {
        using var context = _veritabani.ContextOlustur();

        var islem = await new IslemRepository(context).GetirAsync(5);

        Assert.NotNull(islem);
        Assert.Equal("Raporlama Ekranı", islem.Ad);
        Assert.Equal(3, islem.Zorluk);
    }

    [Fact]
    public async Task GetirAsync_OlmayanId_NullDondurur()
    {
        using var context = _veritabani.ContextOlustur();

        var islem = await new IslemRepository(context).GetirAsync(999);

        Assert.Null(islem);
    }
}
