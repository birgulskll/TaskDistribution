using TaskDistribution.Business.Helpers;
using TaskDistribution.Business.Models;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

/// <summary>
/// Görevleri personellere adil şekilde dağıtan algoritma.
/// Veritabanını bilmez; kendisine verilen personel listesi üzerinde çalışır.
/// Bu sayede test etmek çok kolaydır.
/// </summary>
public class AtamaService : IAtamaService
{
    public AtamaSonucu PersonelSec(int zorluk, IEnumerable<Personel> personeller)
    {
        ZorlukSeviyesi.Dogrula(zorluk);

        // ADIM 1: Yalnızca aktif personeller değerlendirilir.
        var aktifPersoneller = personeller
            .Where(p => p.AktifMi)
            .ToList();

        if (aktifPersoneller.Count == 0)
        {
            return AtamaSonucu.Hata("Sistemde aktif personel bulunmuyor. Görev atanamadı.");
        }

        // ADIM 2: Son görevi yeni görevle aynı zorlukta olanlar elenir.
        // Hiç görevi olmayan personelin son görev zorluğu null'dır, bu yüzden her zaman aday olur.
        var adaylar = aktifPersoneller
            .Where(p => SonGorevZorlugu(p) != zorluk)
            .ToList();

        if (adaylar.Count > 0)
        {
            // ADIM 3: Adaylar kurallara göre sıralanır, en baştaki seçilir.
            var secilen = adaylar
                .OrderBy(p => BuZorluktakiGorevSayisi(p, zorluk))   // 1. Bu zorlukta en az görev alan
                .ThenBy(p => p.Gorevler.Count)                       // 2. Toplam en az görev alan
                .ThenBy(p => SonAtanmaTarihi(p))                     // 3. En uzun süredir görev almayan
                .ThenBy(p => p.Id)                                   // 4. Id'si en küçük olan
                .First();

            return AtamaSonucu.Basari(
                secilen.Id,
                $"Görev {secilen.TamAd} adlı personele atandı.");
        }

        // ADIM 4: Hiç aday kalmadı. Ardışık zorluk kuralı esnetilir,
        // tüm aktif personeller arasından toplam görev sayısı en az olan seçilir.
        var esnetilerekSecilen = aktifPersoneller
            .OrderBy(p => p.Gorevler.Count)
            .ThenBy(p => SonAtanmaTarihi(p))
            .ThenBy(p => p.Id)
            .First();

        return AtamaSonucu.EsnetilerekBasari(
            esnetilerekSecilen.Id,
            $"Uygun aday bulunamadı: tüm aktif personellerin son görevi {zorluk}. zorluk seviyesindeydi. " +
            $"Ardışık zorluk kuralı esnetilerek görev, en az görevi olan {esnetilerekSecilen.TamAd} adlı personele atandı.");
    }

    // ---------------- Yardımcı metotlar ----------------

    private static int? SonGorevZorlugu(Personel personel)
    {
        // "?." operatörü: SonGorev null ise hata vermeden null döndürür.
        return GorevGecmisi.SonGorev(personel)?.Zorluk;
    }

    /// <summary>
    /// Hiç görev almamış personel için DateTime.MinValue (en eski tarih) döner,
    /// böylece tarih sıralamasında en öne geçer.
    /// </summary>
    private static DateTime SonAtanmaTarihi(Personel personel)
    {
        return GorevGecmisi.SonGorev(personel)?.OlusturmaTarihi ?? DateTime.MinValue;
    }

    private static int BuZorluktakiGorevSayisi(Personel personel, int zorluk)
    {
        return personel.Gorevler.Count(g => g.Zorluk == zorluk);
    }
}
