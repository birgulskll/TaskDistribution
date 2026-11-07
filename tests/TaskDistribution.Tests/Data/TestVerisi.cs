using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Data;

/// <summary>
/// Testlerde tekrar tekrar görev nesnesi yazmamak için küçük yardımcı metotlar.
/// </summary>
public static class TestVerisi
{
    public static TaskItem Gorev(int personelId, int islemId, int zorluk, DateTime? tarih = null)
    {
        return new TaskItem
        {
            Baslik = $"Test görevi (işlem {islemId})",
            AtananPersonelId = personelId,
            IslemId = islemId,
            Zorluk = zorluk,
            OlusturmaTarihi = tarih ?? new DateTime(2026, 11, 7)
        };
    }
}
