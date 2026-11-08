using TaskDistribution.Business.Models;
using TaskDistribution.Data.Repositories;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

/// <summary>
/// Personel işlemleri ve kuralları (boş ad olmaz, uzunluk sınırı vb.).
/// </summary>
public class PersonelService : IPersonelService
{
    private readonly IPersonelRepository _personelRepository;

    public PersonelService(IPersonelRepository personelRepository)
    {
        _personelRepository = personelRepository;
    }

    public Task<List<Personel>> TumunuGetirAsync()
    {
        return _personelRepository.TumunuGetirAsync();
    }

    public Task<Personel?> GetirAsync(int id)
    {
        return _personelRepository.GetirAsync(id);
    }

    public async Task<ServisSonucu> EkleAsync(string? ad, string? soyad)
    {
        ad = ad?.Trim() ?? string.Empty;
        soyad = soyad?.Trim() ?? string.Empty;

        string? hata = AdSoyadKontrolEt(ad, soyad);
        if (hata is not null)
        {
            return ServisSonucu.Hata(hata);
        }

        var personel = new Personel { Ad = ad, Soyad = soyad };
        await _personelRepository.EkleAsync(personel);

        return ServisSonucu.Basari($"{personel.TamAd} eklendi.");
    }

    public async Task<ServisSonucu> GuncelleAsync(int id, string? ad, string? soyad)
    {
        ad = ad?.Trim() ?? string.Empty;
        soyad = soyad?.Trim() ?? string.Empty;

        string? hata = AdSoyadKontrolEt(ad, soyad);
        if (hata is not null)
        {
            return ServisSonucu.Hata(hata);
        }

        var personel = await _personelRepository.GetirAsync(id);
        if (personel is null)
        {
            return ServisSonucu.Hata("Personel bulunamadı.");
        }

        personel.Ad = ad;
        personel.Soyad = soyad;
        await _personelRepository.GuncelleAsync(personel);

        return ServisSonucu.Basari($"{personel.TamAd} güncellendi.");
    }

    public async Task<ServisSonucu> AktiflikDurumunuDegistirAsync(int id)
    {
        var personel = await _personelRepository.GetirAsync(id);
        if (personel is null)
        {
            return ServisSonucu.Hata("Personel bulunamadı.");
        }

        personel.AktifMi = !personel.AktifMi;
        await _personelRepository.GuncelleAsync(personel);

        return personel.AktifMi
            ? ServisSonucu.Basari($"{personel.TamAd} aktif yapıldı.")
            : ServisSonucu.Basari($"{personel.TamAd} pasif yapıldı. Bu personele artık yeni görev atanmayacak.");
    }

    public async Task<ServisSonucu> SilAsync(int id)
    {
        var personel = await _personelRepository.GetirAsync(id);
        if (personel is null)
        {
            return ServisSonucu.Hata("Personel bulunamadı.");
        }

        // Kural: görev geçmişi olan personel silinmez. Silinseydi geçmiş görevler sahipsiz kalırdı
        // ve görev dağılımı tablosu yanlış olurdu. (Veritabanındaki Restrict kuralı da buna izin vermez.)
        if (await _personelRepository.GoreviVarMiAsync(id))
        {
            return ServisSonucu.Hata(
                $"{personel.TamAd} adlı personelin görev geçmişi olduğu için silinemez. Bunun yerine pasif yapabilirsiniz.");
        }

        await _personelRepository.SilAsync(personel);

        return ServisSonucu.Basari($"{personel.TamAd} silindi.");
    }

    /// <summary>
    /// Ad ve soyad kurallarını kontrol eder. Sorun varsa hata mesajını, yoksa null döndürür.
    /// </summary>
    private static string? AdSoyadKontrolEt(string ad, string soyad)
    {
        if (ad.Length == 0)
        {
            return "Ad boş olamaz.";
        }

        if (soyad.Length == 0)
        {
            return "Soyad boş olamaz.";
        }

        if (ad.Length > AlanUzunluklari.PersonelAd)
        {
            return $"Ad en fazla {AlanUzunluklari.PersonelAd} karakter olabilir.";
        }

        if (soyad.Length > AlanUzunluklari.PersonelSoyad)
        {
            return $"Soyad en fazla {AlanUzunluklari.PersonelSoyad} karakter olabilir.";
        }

        return null;
    }
}
