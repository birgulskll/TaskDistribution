using TaskDistribution.Business.Models;
using TaskDistribution.Business.Services;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Tests.Web;

/// <summary>GorevController testleri için sahte görev servisi.</summary>
public class SahteGorevService : IGorevService
{
    public ServisSonucu DonecekSonuc { get; set; } = ServisSonucu.Basari("Görev atandı.");

    public int OlusturCagrilmaSayisi { get; private set; }

    /// <summary>Servise en son hangi işlem Id'si gönderildi?</summary>
    public int? SonGelenIslemId { get; private set; }

    public Task<List<TaskItem>> TumunuGetirAsync() => Task.FromResult(new List<TaskItem>());

    public Task<ServisSonucu> OlusturAsync(string? baslik, string? aciklama, int islemId)
    {
        OlusturCagrilmaSayisi++;
        SonGelenIslemId = islemId;
        return Task.FromResult(DonecekSonuc);
    }
}

/// <summary>GorevController testleri için sahte işlem servisi (3 işlem döndürür).</summary>
public class SahteIslemService : IIslemService
{
    private readonly List<Islem> _islemler = new()
    {
        new Islem { Id = 1, Ad = "Kolay iş", Zorluk = 1 },
        new Islem { Id = 2, Ad = "Orta iş", Zorluk = 3 },
        new Islem { Id = 3, Ad = "Zor iş", Zorluk = 5 }
    };

    public Task<List<Islem>> TumunuGetirAsync() => Task.FromResult(_islemler);

    public Task<Islem?> GetirAsync(int id) => Task.FromResult(_islemler.FirstOrDefault(i => i.Id == id));
}
