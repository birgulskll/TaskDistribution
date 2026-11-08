using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Helpers;

/// <summary>
/// Personelin görev geçmişiyle ilgili ortak kurallar.
/// AtamaService ve OzetService aynı "son görev" tanımını kullansın diye tek yerde duruyor.
/// </summary>
internal static class GorevGecmisi
{
    /// <summary>
    /// Personelin en son aldığı görev. Önce tarihe, tarihler eşitse Id'ye bakılır.
    /// Hiç görevi yoksa null döner.
    /// </summary>
    public static TaskItem? SonGorev(Personel personel)
    {
        return personel.Gorevler
            .OrderByDescending(g => g.OlusturmaTarihi)
            .ThenByDescending(g => g.Id)
            .FirstOrDefault();
    }
}
