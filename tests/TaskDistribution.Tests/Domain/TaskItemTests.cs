using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Domain;

public class TaskItemTests
{
    [Fact]
    public void Zorluk_GecerliDeger_Atanir()
    {
        var gorev = new TaskItem();

        gorev.Zorluk = 4;

        Assert.Equal(4, gorev.Zorluk);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Zorluk_GecersizDeger_HataFirlatir(int zorluk)
    {
        var gorev = new TaskItem();

        Assert.Throws<ArgumentOutOfRangeException>(() => gorev.Zorluk = zorluk);
    }

    [Fact]
    public void Aciklama_BosBirakilabilir()
    {
        var gorev = new TaskItem { Baslik = "Rapor hazırla" };

        Assert.Null(gorev.Aciklama);
    }

    [Fact]
    public void Gorev_IslemVePersonelIleIliskilendirilebilir()
    {
        // Arrange
        var islem = new Islem { Id = 3, Ad = "Raporlama Ekranı", Zorluk = 3 };
        var personel = new Personel { Id = 7, Ad = "Ali", Soyad = "Kaya" };

        // Act
        var gorev = new TaskItem
        {
            Baslik = "Aylık satış raporu",
            Islem = islem,
            IslemId = islem.Id,
            Zorluk = islem.Zorluk,
            AtananPersonel = personel,
            AtananPersonelId = personel.Id
        };

        // Assert
        Assert.Equal(3, gorev.IslemId);
        Assert.Equal(3, gorev.Zorluk);
        Assert.Equal("Ali Kaya", gorev.AtananPersonel.TamAd);
    }
}
