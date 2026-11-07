using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data;

/// <summary>
/// Uygulama ile veritabanı arasındaki köprü.
/// Her DbSet, veritabanındaki bir tabloyu temsil eder.
/// </summary>
public class AppDbContext : DbContext
{
    // Hangi veritabanına (SQLite, dosya yolu vb.) bağlanılacağı bilgisi dışarıdan "options" ile gelir.
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Personel> Personeller => Set<Personel>();

    public DbSet<Islem> Islemler => Set<Islem>();

    public DbSet<TaskItem> Gorevler => Set<TaskItem>();

    /// <summary>
    /// Tabloların nasıl oluşturulacağını EF Core'a anlattığımız yer.
    /// Ayarları tek tek buraya yazmak yerine Configurations klasöründeki sınıflardan okuyoruz.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Bu projedeki IEntityTypeConfiguration sınıflarının hepsini bulup uygular.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
