using Microsoft.Extensions.DependencyInjection;
using TaskDistribution.Business.Services;
using TaskDistribution.Data;

namespace TaskDistribution.Business;

/// <summary>
/// Business katmanının (ve altındaki Data katmanının) sınıflarını kaydeder.
/// Web projesi sadece bu metodu çağırır; Data katmanının detaylarını bilmesine gerek kalmaz.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services, string connectionString)
    {
        // Önce bir alt katmanı kaydet
        services.AddDataLayer(connectionString);

        // AtamaService hiçbir veri tutmuyor (sadece hesaplama yapıyor).
        // Bu yüzden uygulama boyunca TEK bir tane yeterli: Singleton.
        services.AddSingleton<IAtamaService, AtamaService>();

        // Bu servisler repository'leri, onlar da DbContext'i kullanıyor.
        // DbContext her istek için ayrı olduğundan bunlar da istek başına bir tane: Scoped.
        services.AddScoped<IPersonelService, PersonelService>();
        services.AddScoped<IIslemService, IslemService>();
        services.AddScoped<IGorevService, GorevService>();
        services.AddScoped<IOzetService, OzetService>();

        return services;
    }

    /// <summary>
    /// Veritabanını kullanıma hazırlar (migration'ları uygular).
    /// Web katmanı Data katmanını doğrudan çağırmasın diye araya konmuş bir kapı.
    /// </summary>
    public static void VeritabaniniHazirla(this IServiceProvider serviceProvider)
    {
        serviceProvider.VeritabaniniGuncelle();
    }
}
