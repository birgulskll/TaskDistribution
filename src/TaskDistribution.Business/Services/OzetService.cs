using TaskDistribution.Business.Helpers;
using TaskDistribution.Business.Models;
using TaskDistribution.Data.Repositories;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

/// <summary>
/// Ana sayfadaki görev dağılımı tablosunun verisini hazırlar.
/// </summary>
public class OzetService : IOzetService
{
    private readonly IPersonelRepository _personelRepository;

    public OzetService(IPersonelRepository personelRepository)
    {
        _personelRepository = personelRepository;
    }

    public async Task<List<PersonelOzeti>> PersonelOzetleriniGetirAsync()
    {
        var personeller = await _personelRepository.TumunuGorevleriyleGetirAsync();

        return personeller
            .Select(OzetOlustur)
            .ToList();
    }

    private static PersonelOzeti OzetOlustur(Personel personel)
    {
        // Görevleri zorluğa göre grupla ve say. Örnek sonuç: { 1: 2, 3: 1 }
        var gruplar = personel.Gorevler
            .GroupBy(g => g.Zorluk)
            .ToDictionary(grup => grup.Key, grup => grup.Count());

        // 1'den 5'e kadar her zorluk için bir değer olsun; hiç görev yoksa 0.
        var zorlukSayilari = new Dictionary<int, int>();
        for (int zorluk = ZorlukSeviyesi.EnDusuk; zorluk <= ZorlukSeviyesi.EnYuksek; zorluk++)
        {
            zorlukSayilari[zorluk] = gruplar.GetValueOrDefault(zorluk, 0);
        }

        // AtamaService ile aynı "son görev" kuralı
        var sonGorev = GorevGecmisi.SonGorev(personel);

        return new PersonelOzeti
        {
            PersonelId = personel.Id,
            TamAd = personel.TamAd,
            AktifMi = personel.AktifMi,
            ZorlukSayilari = zorlukSayilari,
            ToplamGorev = personel.Gorevler.Count,
            SonGorevZorlugu = sonGorev?.Zorluk,
            SonAtanmaTarihi = sonGorev?.OlusturmaTarihi
        };
    }
}
