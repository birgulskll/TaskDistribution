using TaskDistribution.Business.Models;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

public interface IGorevService
{
    Task<List<TaskItem>> TumunuGetirAsync();

    /// <summary>
    /// Yeni görev oluşturur ve otomatik olarak bir personele atar.
    /// Dikkat: zorluk parametresi YOK. Zorluk her zaman seçilen işlemden alınır.
    /// </summary>
    Task<ServisSonucu> OlusturAsync(string? baslik, string? aciklama, int islemId);
}
