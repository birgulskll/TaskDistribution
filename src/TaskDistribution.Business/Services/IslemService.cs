using TaskDistribution.Data.Repositories;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

/// <summary>
/// İşlemler sadece okunur (seed data ile gelir). Bu servis, Web katmanının
/// repository'yi doğrudan görmemesi için aradaki kapıdır.
/// </summary>
public class IslemService : IIslemService
{
    private readonly IIslemRepository _islemRepository;

    public IslemService(IIslemRepository islemRepository)
    {
        _islemRepository = islemRepository;
    }

    public Task<List<Islem>> TumunuGetirAsync()
    {
        return _islemRepository.TumunuGetirAsync();
    }

    public Task<Islem?> GetirAsync(int id)
    {
        return _islemRepository.GetirAsync(id);
    }
}
