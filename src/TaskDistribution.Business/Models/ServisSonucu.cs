namespace TaskDistribution.Business.Models;

/// <summary>
/// Ekleme, güncelleme, görev oluşturma gibi işlemlerin sonucu.
/// Web katmanı bu nesneye bakarak kullanıcıya yeşil (başarılı), sarı (uyarı)
/// veya kırmızı (hata) mesaj gösterecek.
/// </summary>
public class ServisSonucu
{
    private ServisSonucu(bool basarili, bool uyariVar, string mesaj)
    {
        Basarili = basarili;
        UyariVar = uyariVar;
        Mesaj = mesaj;
    }

    /// <summary>İşlem gerçekleşti mi?</summary>
    public bool Basarili { get; }

    /// <summary>İşlem gerçekleşti ama kullanıcının bilmesi gereken bir durum var mı? (örn. kural esnetildi)</summary>
    public bool UyariVar { get; }

    public string Mesaj { get; }

    public static ServisSonucu Basari(string mesaj) => new(true, false, mesaj);

    public static ServisSonucu Uyari(string mesaj) => new(true, true, mesaj);

    public static ServisSonucu Hata(string mesaj) => new(false, false, mesaj);
}
