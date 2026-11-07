using TaskDistribution.Business.Models;
using TaskDistribution.Business.Services;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Web;

/// <summary>
/// Controller testleri için sahte (fake) servis. Veritabanına gitmez;
/// ne döneceğini test belirler ve hangi metodun çağrıldığını kaydeder.
/// </summary>
public class SahtePersonelService : IPersonelService
{
    public List<Personel> Personeller { get; } = new();

    /// <summary>Ekle/Güncelle/AktiflikDeğiştir çağrıldığında dönecek sonuç.</summary>
    public ServisSonucu DonecekSonuc { get; set; } = ServisSonucu.Basari("İşlem başarılı.");

    public int EkleCagrilmaSayisi { get; private set; }

    public Task<List<Personel>> TumunuGetirAsync() => Task.FromResult(Personeller);

    public Task<Personel?> GetirAsync(int id) => Task.FromResult(Personeller.FirstOrDefault(p => p.Id == id));

    public Task<ServisSonucu> EkleAsync(string? ad, string? soyad)
    {
        EkleCagrilmaSayisi++;
        return Task.FromResult(DonecekSonuc);
    }

    public Task<ServisSonucu> GuncelleAsync(int id, string? ad, string? soyad) => Task.FromResult(DonecekSonuc);

    public Task<ServisSonucu> AktiflikDurumunuDegistirAsync(int id) => Task.FromResult(DonecekSonuc);
}
