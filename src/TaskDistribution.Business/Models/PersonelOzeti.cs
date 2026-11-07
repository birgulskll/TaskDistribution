namespace TaskDistribution.Business.Models;

/// <summary>
/// Özet tablosunun bir satırı: bir personelin görev dağılımı.
/// Bu tablo, atama algoritmasının adil çalışıp çalışmadığını gözle görmeyi sağlar.
/// </summary>
public class PersonelOzeti
{
    public int PersonelId { get; init; }

    public string TamAd { get; init; } = string.Empty;

    public bool AktifMi { get; init; }

    /// <summary>
    /// Zorluk seviyesine göre görev sayısı. Örnek: ZorlukSayilari[3] = 3. zorluktaki görev sayısı.
    /// 1'den 5'e kadar her zorluk için bir değer vardır (hiç görev yoksa 0).
    /// </summary>
    public IReadOnlyDictionary<int, int> ZorlukSayilari { get; init; } = new Dictionary<int, int>();

    public int ToplamGorev { get; init; }

    /// <summary>Son görevin zorluğu. Algoritma, yeni görev aynı zorluktaysa bu kişiyi eler. Görevi yoksa null.</summary>
    public int? SonGorevZorlugu { get; init; }

    public DateTime? SonAtanmaTarihi { get; init; }
}
