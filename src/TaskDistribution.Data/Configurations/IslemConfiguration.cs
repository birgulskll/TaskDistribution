using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Configurations;

/// <summary>
/// Islemler tablosunun ayarları (Fluent API).
/// </summary>
public class IslemConfiguration : IEntityTypeConfiguration<Islem>
{
    public void Configure(EntityTypeBuilder<Islem> builder)
    {
        // Veritabanı seviyesinde ikinci bir güvenlik: Zorluk 1-5 dışında olamaz.
        builder.ToTable("Islemler", tablo => tablo.HasCheckConstraint(
            "CK_Islemler_Zorluk",
            $"\"Zorluk\" BETWEEN {ZorlukSeviyesi.EnDusuk} AND {ZorlukSeviyesi.EnYuksek}"));

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Ad)
            .IsRequired()
            .HasMaxLength(100);

        // Aynı isimde iki işlem olmasın.
        builder.HasIndex(i => i.Ad)
            .IsUnique();

        builder.Property(i => i.Zorluk)
            .IsRequired();

        // Başlangıç verisi: zorlukları 1-5 arasında dağılmış 8 işlem.
        builder.HasData(
            new Islem { Id = 1, Ad = "Doküman Güncelleme", Zorluk = 1 },
            new Islem { Id = 2, Ad = "Hata Kaydı İnceleme", Zorluk = 1 },
            new Islem { Id = 3, Ad = "Arayüz Düzenleme", Zorluk = 2 },
            new Islem { Id = 4, Ad = "Birim Test Yazma", Zorluk = 2 },
            new Islem { Id = 5, Ad = "Raporlama Ekranı", Zorluk = 3 },
            new Islem { Id = 6, Ad = "Veritabanı Optimizasyonu", Zorluk = 4 },
            new Islem { Id = 7, Ad = "Entegrasyon Geliştirme", Zorluk = 4 },
            new Islem { Id = 8, Ad = "Mimari Tasarım", Zorluk = 5 });
    }
}
