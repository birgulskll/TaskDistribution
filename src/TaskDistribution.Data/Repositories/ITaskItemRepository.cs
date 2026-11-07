using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

/// <summary>
/// Görev verisine erişim. Görev tamamlama ilk sürümde olmadığı için güncelleme/silme yok.
/// </summary>
public interface ITaskItemRepository
{
    /// <summary>Tüm görevler, işlem ve personel bilgisiyle birlikte, en yeni görev en üstte.</summary>
    Task<List<TaskItem>> TumunuGetirAsync();

    Task EkleAsync(TaskItem gorev);
}
