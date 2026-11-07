using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Configurations;

/// <summary>
/// Personeller tablosunun ayarları (Fluent API).
/// </summary>
public class PersonelConfiguration : IEntityTypeConfiguration<Personel>
{
    public void Configure(EntityTypeBuilder<Personel> builder)
    {
        builder.ToTable("Personeller");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Ad)
            .IsRequired()
            .HasMaxLength(AlanUzunluklari.PersonelAd);

        builder.Property(p => p.Soyad)
            .IsRequired()
            .HasMaxLength(AlanUzunluklari.PersonelSoyad);

        // Not: AktifMi için HasDefaultValue(true) KULLANMIYORUZ. EF Core, false değerini
        // "boş" sanıp veritabanı varsayılanını (true) yazardı; pasif personel eklenemezdi.
        // Varsayılan değer zaten Personel sınıfında "= true" olarak verildi.
        builder.Property(p => p.AktifMi)
            .IsRequired();

        // TamAd sadece kodda hesaplanan bir değer; veritabanında sütunu olmasın.
        builder.Ignore(p => p.TamAd);

        // Başlangıç verisi (seed data). Id'ler sabit olmak zorunda, çünkü EF Core
        // her migration'da bu kayıtları Id'lerine bakarak karşılaştırır.
        builder.HasData(
            new Personel { Id = 1, Ad = "Ayşe", Soyad = "Yılmaz", AktifMi = true },
            new Personel { Id = 2, Ad = "Mehmet", Soyad = "Kaya", AktifMi = true },
            new Personel { Id = 3, Ad = "Zeynep", Soyad = "Demir", AktifMi = true },
            new Personel { Id = 4, Ad = "Ali", Soyad = "Çelik", AktifMi = true },
            new Personel { Id = 5, Ad = "Elif", Soyad = "Şahin", AktifMi = true },
            new Personel { Id = 6, Ad = "Burak", Soyad = "Arslan", AktifMi = true },
            new Personel { Id = 7, Ad = "Selin", Soyad = "Koç", AktifMi = true },
            new Personel { Id = 8, Ad = "Emre", Soyad = "Aydın", AktifMi = true });
    }
}
