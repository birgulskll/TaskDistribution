using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Domain;

public class PersonelTests
{
    [Fact]
    public void YeniPersonel_VarsayilanOlarakAktiftir()
    {
        // Arrange & Act
        var personel = new Personel();

        // Assert
        Assert.True(personel.AktifMi);
    }

    [Fact]
    public void YeniPersonel_GorevListesiBosAmaNullDegil()
    {
        var personel = new Personel();

        Assert.NotNull(personel.Gorevler);
        Assert.Empty(personel.Gorevler);
    }

    [Fact]
    public void TamAd_AdVeSoyadiBirlestirir()
    {
        // Arrange
        var personel = new Personel { Ad = "Ayşe", Soyad = "Yılmaz" };

        // Act
        string tamAd = personel.TamAd;

        // Assert
        Assert.Equal("Ayşe Yılmaz", tamAd);
    }
}
