using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Configurations;

/// <summary>
/// Gorevler tablosunun ayarları ve ilişkileri (Fluent API).
/// </summary>
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.ToTable("Gorevler", tablo => tablo.HasCheckConstraint(
            "CK_Gorevler_Zorluk",
            $"\"Zorluk\" BETWEEN {ZorlukSeviyesi.EnDusuk} AND {ZorlukSeviyesi.EnYuksek}"));

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Baslik)
            .IsRequired()
            .HasMaxLength(AlanUzunluklari.GorevBaslik);

        builder.Property(g => g.Aciklama)
            .HasMaxLength(AlanUzunluklari.GorevAciklama);   // IsRequired yok: açıklama boş olabilir

        builder.Property(g => g.Zorluk)
            .IsRequired();

        builder.Property(g => g.OlusturmaTarihi)
            .IsRequired();

        // İlişki 1: Her görevin bir işlemi var, bir işlemin çok görevi olabilir.
        builder.HasOne(g => g.Islem)
            .WithMany(i => i.Gorevler)
            .HasForeignKey(g => g.IslemId)
            .OnDelete(DeleteBehavior.Restrict);   // Görevi olan işlem silinemez

        // İlişki 2: Her görev bir personele atanır, bir personelin çok görevi olabilir.
        builder.HasOne(g => g.AtananPersonel)
            .WithMany(p => p.Gorevler)
            .HasForeignKey(g => g.AtananPersonelId)
            .OnDelete(DeleteBehavior.Restrict);   // Görevi olan personel silinemez
    }
}
