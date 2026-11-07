namespace TaskDistribution.Domain.Constants;

/// <summary>
/// Metin alanlarının en fazla kaç karakter olabileceği.
/// Hem veritabanı ayarları (Data) hem de doğrulama kuralları (Business) bu değerleri kullanır.
/// </summary>
public static class AlanUzunluklari
{
    public const int PersonelAd = 50;
    public const int PersonelSoyad = 50;
    public const int IslemAd = 100;
    public const int GorevBaslik = 200;
    public const int GorevAciklama = 1000;
}
