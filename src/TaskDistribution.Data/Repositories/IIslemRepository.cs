using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

/// <summary>
/// İşlem verisine erişim. İşlemler sadece seed data ile geldiği için ekleme/güncelleme yok.
/// </summary>
public interface IIslemRepository
{
    /// <summary>Tüm işlemler, önce zorluğa sonra ada göre sıralı.</summary>
    Task<List<Islem>> TumunuGetirAsync();

    /// <summary>Id ile tek işlem getirir; bulamazsa null döner.</summary>
    Task<Islem?> GetirAsync(int id);
}
