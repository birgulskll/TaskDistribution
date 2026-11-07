using TaskDistribution.Business.Models;

namespace TaskDistribution.Business.Services;

public interface IOzetService
{
    /// <summary>Her personelin (aktif ve pasif) zorluk bazında görev dağılımı.</summary>
    Task<List<PersonelOzeti>> PersonelOzetleriniGetirAsync();
}
