using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TaskDistribution.Data;

/// <summary>
/// Sadece "dotnet ef" komutları (migration oluşturma, veritabanı güncelleme) için kullanılır.
/// Uygulama çalışırken bu sınıf devreye girmez; Web projesi kendi ayarlarını Aşama 7'de verecek.
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
