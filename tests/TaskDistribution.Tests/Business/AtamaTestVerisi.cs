using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Business;

/// <summary>
/// Atama testlerinde senaryoları kısa ve okunaklı kurmak için yardımcı metotlar.
/// </summary>
public static class AtamaTestVerisi
{
    public static Personel Personel(int id, bool aktifMi = true)
    {
        return new Personel { Id = id, Ad = $"Personel{id}", Soyad = "Test", AktifMi = aktifMi };
    }

    /// <summary>
    /// Personele, Kasım 2026'nın verilen gününde, verilen zorlukta bir görev ekler.
    /// </summary>
    public static void GorevEkle(Personel personel, int zorluk, int gun)
    {
        personel.Gorevler.Add(new TaskItem
        {
            Id = personel.Gorevler.Count + 1,
            Baslik = "Test görevi",
            Zorluk = zorluk,
            AtananPersonelId = personel.Id,
            OlusturmaTarihi = new DateTime(2026, 11, gun)
        });
    }
}
