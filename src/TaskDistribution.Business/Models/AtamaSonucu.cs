namespace TaskDistribution.Business.Models;

/// <summary>
/// Atama algoritmasının cevabı: kim seçildi, kural esnetildi mi, kullanıcıya ne söylenecek?
/// Hata fırlatmak yerine bu nesneyi döndürüyoruz; böylece "aktif personel yok" gibi
/// beklenen durumlar normal bir sonuç olarak ele alınabiliyor.
/// </summary>
public class AtamaSonucu
{
    // Constructor private: dışarıdan "new AtamaSonucu(...)" yazılamaz.
    // Nesne sadece aşağıdaki üç hazır metotla oluşturulur, böylece çelişkili bir sonuç
    // (örneğin hem başarısız hem de PersonelId dolu) oluşturmak imkânsız olur.
    private AtamaSonucu(bool basarili, int? personelId, bool kuralEsnetildi, string mesaj)
    {
        Basarili = basarili;
        PersonelId = personelId;
        KuralEsnetildi = kuralEsnetildi;
        Mesaj = mesaj;
    }

    /// <summary>Bir personel seçilebildi mi?</summary>
    public bool Basarili { get; }

    /// <summary>Seçilen personelin Id'si. Başarısız sonuçta null'dır.</summary>
    public int? PersonelId { get; }

    /// <summary>Ardışık zorluk kuralı esnetilerek mi seçildi? (true ise kullanıcıya uyarı gösterilir)</summary>
    public bool KuralEsnetildi { get; }

    /// <summary>Kullanıcıya gösterilecek açıklama.</summary>
    public string Mesaj { get; }

    public static AtamaSonucu Basari(int personelId, string mesaj)
        => new(true, personelId, false, mesaj);

    public static AtamaSonucu EsnetilerekBasari(int personelId, string uyariMesaji)
        => new(true, personelId, true, uyariMesaji);

    public static AtamaSonucu Hata(string mesaj)
        => new(false, null, false, mesaj);
}
