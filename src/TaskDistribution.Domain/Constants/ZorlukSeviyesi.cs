namespace TaskDistribution.Domain.Constants;

/// <summary>
/// Zorluk seviyesiyle ilgili kuralların tek bir yerde toplandığı sınıf.
/// "1 ile 5 arası" bilgisi projede sadece burada yazılır; diğer her yer buradan okur.
/// </summary>
public static class ZorlukSeviyesi
{
    public const int EnDusuk = 1;
    public const int EnYuksek = 5;

    /// <summary>
    /// Verilen değerin geçerli bir zorluk seviyesi olup olmadığını söyler.
    /// </summary>
    public static bool GecerliMi(int zorluk)
    {
        return zorluk >= EnDusuk && zorluk <= EnYuksek;
    }

    /// <summary>
    /// Değer geçersizse hata fırlatır, geçerliyse aynı değeri geri döndürür.
    /// Entity'lerdeki Zorluk property'leri bu metodu kullanır.
    /// </summary>
    public static int Dogrula(int zorluk)
    {
        if (!GecerliMi(zorluk))
        {
            throw new ArgumentOutOfRangeException(
                nameof(zorluk),
                zorluk,
                $"Zorluk seviyesi {EnDusuk} ile {EnYuksek} arasında olmalıdır.");
        }

        return zorluk;
    }
}
