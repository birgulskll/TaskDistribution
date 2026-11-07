using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

public interface IIslemService
{
    Task<List<Islem>> TumunuGetirAsync();

    Task<Islem?> GetirAsync(int id);
}
