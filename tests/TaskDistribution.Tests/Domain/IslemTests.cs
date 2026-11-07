using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Domain;

public class IslemTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    public void Zorluk_GecerliDeger_Atanir(int zorluk)
    {
        // Arrange
        var islem = new Islem();

        // Act
        islem.Zorluk = zorluk;

        // Assert
        Assert.Equal(zorluk, islem.Zorluk);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    public void Zorluk_GecersizDeger_HataFirlatir(int zorluk)
    {
        var islem = new Islem();

        Assert.Throws<ArgumentOutOfRangeException>(() => islem.Zorluk = zorluk);
    }
}
