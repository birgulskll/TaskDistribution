using Microsoft.Extensions.DependencyInjection;
using TaskDistribution.Business;
using TaskDistribution.Business.Services;

namespace TaskDistribution.Tests.Business;

public class DependencyInjectionTests
{
    [Fact]
    public void AddBusinessLayer_TumServisleriEksiksizKaydeder()
    {
        // Arrange: Web projesinin Aşama 7'de yapacağı kaydın aynısı
        var services = new ServiceCollection();
        services.AddBusinessLayer("DataSource=:memory:");

        // ValidateOnBuild: kayıtlarda eksik bir bağımlılık varsa hemen hata ver.
        // ValidateScopes: Scoped bir servis yanlışlıkla Singleton içine konmuşsa hata ver.
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        });

        // Act: bir web isteğini taklit eden "scope" aç ve servisleri iste
        using var scope = provider.CreateScope();

        // Assert
        Assert.IsType<PersonelService>(scope.ServiceProvider.GetRequiredService<IPersonelService>());
        Assert.IsType<IslemService>(scope.ServiceProvider.GetRequiredService<IIslemService>());
        Assert.IsType<GorevService>(scope.ServiceProvider.GetRequiredService<IGorevService>());
        Assert.IsType<AtamaService>(scope.ServiceProvider.GetRequiredService<IAtamaService>());
        Assert.IsType<OzetService>(scope.ServiceProvider.GetRequiredService<IOzetService>());
    }

    [Fact]
    public void AtamaService_SingletonOldugu_IcinHepAyniNesneDoner()
    {
        var services = new ServiceCollection();
        services.AddBusinessLayer("DataSource=:memory:");
        using var provider = services.BuildServiceProvider();

        using var scope1 = provider.CreateScope();
        using var scope2 = provider.CreateScope();

        // Singleton: farklı isteklerde bile aynı nesne
        Assert.Same(
            scope1.ServiceProvider.GetRequiredService<IAtamaService>(),
            scope2.ServiceProvider.GetRequiredService<IAtamaService>());

        // Scoped: farklı isteklerde farklı nesne
        Assert.NotSame(
            scope1.ServiceProvider.GetRequiredService<IGorevService>(),
            scope2.ServiceProvider.GetRequiredService<IGorevService>());
    }
}
