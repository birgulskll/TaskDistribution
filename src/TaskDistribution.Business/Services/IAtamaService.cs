using TaskDistribution.Business.Models;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Business.Services;

public interface IAtamaService
{
    /// <summary>
    /// Verilen zorluktaki yeni görev için en uygun personeli seçer.
    /// Personellerin Gorevler listesi dolu (görev geçmişiyle birlikte) gelmelidir.
    /// </summary>
    AtamaSonucu PersonelSec(int zorluk, IEnumerable<Personel> personeller);
}
