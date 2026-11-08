using Microsoft.Extensions.DependencyInjection;
using TaskDistribution.Business.Services;
using TaskDistribution.Data;

namespace TaskDistribution.Business;

/// <summary>
/// Business ve Data katmanlarının servislerini kaydeder. Web projesi yalnızca bu metodu çağırır.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, string connectionString)
    {
        services.AddDataLayer(connectionString);

        // Durum tutmayan algoritma: uygulama boyunca tek örnek yeterli.
        services.AddSingleton<IAtamaService, AtamaService>();

        // DbContext'e bağlı servisler: istek başına bir örnek.
        services.AddScoped<IPersonelService, PersonelService>();
        services.AddScoped<IIslemService, IslemService>();
        services.AddScoped<IGorevService, GorevService>();
        services.AddScoped<IOzetService, OzetService>();

        return services;
    }

    /// <summary>
    /// Bekleyen migration'ları uygular. Web katmanı Data katmanını doğrudan çağırmasın diye buradan sunulur.
    /// </summary>
    public static void VeritabaniniHazirla(this IServiceProvider serviceProvider)
    {
        serviceProvider.VeritabaniniGuncelle();
    }
}
