namespace TaskDistribution.Domain.Entities;

/// <summary>
/// Görev atanabilecek bir çalışanı temsil eder.
/// </summary>
public class Personel
{
    public int Id { get; set; }

    public string Ad { get; set; } = string.Empty;

    public string Soyad { get; set; } = string.Empty;

    /// <summary>
    /// Pasif personele yeni görev atanmaz. Yeni eklenen personel varsayılan olarak aktiftir.
    /// </summary>
    public bool AktifMi { get; set; } = true;

    /// <summary>
    /// Bu personele atanmış görevler.
    /// </summary>
    public ICollection<TaskItem> Gorevler { get; set; } = new List<TaskItem>();

    /// <summary>
    /// Ekranlarda göstermek için "Ad Soyad". Hesaplanan bir değerdir, veritabanına kaydedilmez.
    /// </summary>
    public string TamAd => $"{Ad} {Soyad}";
}
