using TaskDistribution.Business.Models;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

public interface IPersonelService
{
    Task<List<Personel>> TumunuGetirAsync();

    Task<Personel?> GetirAsync(int id);

    Task<ServisSonucu> EkleAsync(string? ad, string? soyad);

    Task<ServisSonucu> GuncelleAsync(int id, string? ad, string? soyad);

    /// <summary>Aktif personeli pasif, pasif personeli aktif yapar.</summary>
    Task<ServisSonucu> AktiflikDurumunuDegistirAsync(int id);
}
