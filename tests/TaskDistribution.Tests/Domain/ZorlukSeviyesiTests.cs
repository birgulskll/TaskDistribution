using TaskDistribution.Domain.Constants;

namespace TaskDistribution.Tests.Domain;

public class ZorlukSeviyesiTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(3)]
    [InlineData(5)]
    public void GecerliMi_1ile5ArasindakiDegerler_TrueDoner(int zorluk)
    {
        // Act
        bool sonuc = ZorlukSeviyesi.GecerliMi(zorluk);

        // Assert
        Assert.True(sonuc);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void GecerliMi_AralikDisindakiDegerler_FalseDoner(int zorluk)
    {
        bool sonuc = ZorlukSeviyesi.GecerliMi(zorluk);

        Assert.False(sonuc);
    }

    [Fact]
    public void Dogrula_GecersizDeger_HataFirlatir()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ZorlukSeviyesi.Dogrula(7));
    }
}
