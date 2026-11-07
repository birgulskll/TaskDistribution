using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TaskDistribution.Data;

namespace TaskDistribution.Tests.Data;

/// <summary>
/// Testler için bellekte (RAM'de) geçici bir SQLite veritabanı oluşturur.
/// Gerçek taskdistribution.db dosyasına hiç dokunmaz; test bitince veritabanı yok olur.
/// </summary>
public sealed class SqliteTestVeritabani : IDisposable
{
    // ":memory:" veritabanı, bağlantı açık kaldığı sürece yaşar. Bu yüzden bağlantıyı saklıyoruz.
    private readonly SqliteConnection _baglanti;

    public SqliteTestVeritabani()
    {
        _baglanti = new SqliteConnection("DataSource=:memory:");
        _baglanti.Open();

        // Gerçek migration'ları çalıştırıyoruz; böylece migration'ın kendisi de test edilmiş olur.
        using var context = ContextOlustur();
        context.Database.Migrate();
    }

    /// <summary>
    /// Aynı bellek veritabanına bağlı yeni bir DbContext döndürür.
    /// </summary>
    public AppDbContext ContextOlustur()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_baglanti)
            .Options;

        return new AppDbContext(options);
    }

    public void Dispose()
    {
        _baglanti.Dispose();
    }
}
