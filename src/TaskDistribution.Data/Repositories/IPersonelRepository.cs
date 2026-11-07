using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

/// <summary>
/// Personel verisine erişim için "sözleşme". Neyin yapılabileceğini söyler, nasıl yapılacağını söylemez.
/// </summary>
public interface IPersonelRepository
{
    /// <summary>Tüm personeller (aktif ve pasif), ada göre sıralı.</summary>
    Task<List<Personel>> TumunuGetirAsync();

    /// <summary>Sadece aktif personeller, görev geçmişleriyle birlikte. Atama algoritması bunu kullanacak.</summary>
    Task<List<Personel>> AktifleriGorevleriyleGetirAsync();

    /// <summary>Tüm personeller (aktif ve pasif), görev geçmişleriyle birlikte. Özet tablosu bunu kullanacak.</summary>
    Task<List<Personel>> TumunuGorevleriyleGetirAsync();

    /// <summary>Id ile tek personel getirir; bulamazsa null döner.</summary>
    Task<Personel?> GetirAsync(int id);

    Task EkleAsync(Personel personel);

    Task GuncelleAsync(Personel personel);
}
