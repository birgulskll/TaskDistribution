using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data;

/// <summary>
/// EF Core veritabanı bağlamı. Tablo ayarları Configurations klasöründeki sınıflardadır.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Personel> Personeller => Set<Personel>();

    public DbSet<Islem> Islemler => Set<Islem>();

    public DbSet<TaskItem> Gorevler => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
