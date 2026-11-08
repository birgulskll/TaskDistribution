using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskDistribution.Data;

/// <summary>
/// Sadece "dotnet ef" komutları (migration oluşturma, veritabanı güncelleme) için kullanılır.
/// Uygulama çalışırken kullanılmaz; Web projesi bağlantı ayarını appsettings.json dosyasından alır.
/// </summary>
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("Data Source=taskdistribution.db")
            .Options;

        return new AppDbContext(options);
    }
}
