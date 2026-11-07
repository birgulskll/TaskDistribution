using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskDistribution.Data.Repositories;

namespace TaskDistribution.Data;

/// <summary>
/// Data katmanının sınıflarını .NET'in Dependency Injection sistemine tanıtır.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// DbContext'i ve repository'leri kaydeder.
    /// "this IServiceCollection" sayesinde services.AddDataLayer(...) şeklinde çağrılabilir (extension method).
    /// </summary>
    public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
    {
        // AppDbContext istendiğinde SQLite'a bağlı bir tane oluştur.
        // AddDbContext varsayılan olarak "Scoped" kaydeder: her web isteği için bir tane.
        services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

        // "Biri IPersonelRepository isterse ona PersonelRepository ver."
        services.AddScoped<IPersonelRepository, PersonelRepository>();
        services.AddScoped<IIslemRepository, IslemRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();

        return services;
    }

    /// <summary>
    /// Uygulanmamış migration'ları veritabanına uygular. Veritabanı dosyası yoksa oluşturur
    /// ve seed data'yı (8 personel, 8 işlem) ekler. Uygulama açılırken bir kez çağrılır.
    /// </summary>
    public static void VeritabaniniGuncelle(this IServiceProvider serviceProvider)
    {
        // DbContext "Scoped" olduğu için önce geçici bir scope açmamız gerekiyor.
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
}
