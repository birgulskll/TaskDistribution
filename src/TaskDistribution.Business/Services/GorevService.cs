using Microsoft.Extensions.Logging;
using TaskDistribution.Business.Models;
using TaskDistribution.Data.Repositories;
using TaskDistribution.Domain.Constants;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

/// <summary>
/// Görev oluşturma akışını yönetir: kontrol et → zorluğu işlemden al → personel seç → kaydet.
/// Kararı kendisi vermez; personel seçimini AtamaService'e bırakır.
/// </summary>
public class GorevService : IGorevService
{
    private readonly ITaskItemRepository _gorevRepository;
    private readonly IIslemRepository _islemRepository;
    private readonly IPersonelRepository _personelRepository;
    private readonly IAtamaService _atamaService;
    private readonly ILogger<GorevService> _logger;

    public GorevService(
        ITaskItemRepository gorevRepository,
        IIslemRepository islemRepository,
        IPersonelRepository personelRepository,
        IAtamaService atamaService,
        ILogger<GorevService> logger)
    {
        _gorevRepository = gorevRepository;
        _islemRepository = islemRepository;
        _personelRepository = personelRepository;
        _atamaService = atamaService;
        _logger = logger;
    }

    public Task<List<TaskItem>> TumunuGetirAsync()
    {
        return _gorevRepository.TumunuGetirAsync();
    }

    public async Task<ServisSonucu> OlusturAsync(string? baslik, string? aciklama, int islemId)
    {
        // 1. Girdileri temizle ve kontrol et
        baslik = baslik?.Trim() ?? string.Empty;
        aciklama = string.IsNullOrWhiteSpace(aciklama) ? null : aciklama.Trim();

        if (baslik.Length == 0)
        {
            return ServisSonucu.Hata("Görev başlığı boş olamaz.");
        }

        if (baslik.Length > AlanUzunluklari.GorevBaslik)
        {
            return ServisSonucu.Hata($"Görev başlığı en fazla {AlanUzunluklari.GorevBaslik} karakter olabilir.");
        }

        if (aciklama is not null && aciklama.Length > AlanUzunluklari.GorevAciklama)
        {
            return ServisSonucu.Hata($"Açıklama en fazla {AlanUzunluklari.GorevAciklama} karakter olabilir.");
        }

        // 2. İşlemi bul. Zorluk BURADAN alınır; kullanıcı zorluğu hiçbir şekilde gönderemez.
        var islem = await _islemRepository.GetirAsync(islemId);
        if (islem is null)
        {
            return ServisSonucu.Hata("Seçilen işlem bulunamadı.");
        }

        int zorluk = islem.Zorluk;

        // 3. Aktif personelleri görev geçmişleriyle al ve atama algoritmasına sor
        var aktifPersoneller = await _personelRepository.AktifleriGorevleriyleGetirAsync();
        AtamaSonucu atama = _atamaService.PersonelSec(zorluk, aktifPersoneller);

        if (!atama.Basarili)
        {
            // Örneğin "Sistemde aktif personel bulunmuyor." Görev KAYDEDİLMEZ.
            _logger.LogWarning(
                "Görev atanamadı: '{Baslik}' (zorluk {Zorluk}). Neden: {Mesaj}",
                baslik, zorluk, atama.Mesaj);

            return ServisSonucu.Hata(atama.Mesaj);
        }

        // 4. Görevi oluştur ve kaydet
        var gorev = new TaskItem
        {
            Baslik = baslik,
            Aciklama = aciklama,
            IslemId = islem.Id,
            Zorluk = zorluk,
            AtananPersonelId = atama.PersonelId!.Value,   // Basarili ise PersonelId her zaman doludur
            OlusturmaTarihi = DateTime.Now
        };

        await _gorevRepository.EkleAsync(gorev);

        // 5. Kayıt tut (log): sorun çıktığında "hangi görev kime, neden atandı?" sorusunun cevabı
        if (atama.KuralEsnetildi)
        {
            _logger.LogWarning(
                "Görev {GorevId} '{Baslik}' (zorluk {Zorluk}) KURAL ESNETİLEREK personel {PersonelId} kişisine atandı.",
                gorev.Id, gorev.Baslik, gorev.Zorluk, gorev.AtananPersonelId);
        }
        else
        {
            _logger.LogInformation(
                "Görev {GorevId} '{Baslik}' (zorluk {Zorluk}) personel {PersonelId} kişisine atandı.",
                gorev.Id, gorev.Baslik, gorev.Zorluk, gorev.AtananPersonelId);
        }

        // 6. Sonucu bildir: kural esnetildiyse uyarı, değilse başarı
        return atama.KuralEsnetildi
            ? ServisSonucu.Uyari(atama.Mesaj)
            : ServisSonucu.Basari(atama.Mesaj);
    }
}
