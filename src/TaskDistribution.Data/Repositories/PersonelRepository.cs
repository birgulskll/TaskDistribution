using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

/// <summary>
/// IPersonelRepository sözleşmesinin EF Core ile yazılmış hali.
/// </summary>
public class PersonelRepository : IPersonelRepository
{
    private readonly AppDbContext _context;

    // DbContext dışarıdan verilir (constructor injection). Aşama 6'da bunu .NET otomatik yapacak.
    public PersonelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Personel>> TumunuGetirAsync()
    {
        return await _context.Personeller
            .OrderBy(p => p.Ad)
            .ThenBy(p => p.Soyad)
            .ToListAsync();
    }

    public async Task<List<Personel>> AktifleriGorevleriyleGetirAsync()
    {
        return await _context.Personeller
            .Where(p => p.AktifMi)
            .Include(p => p.Gorevler)   // Her personelin görevlerini de aynı sorguda getir
            .OrderBy(p => p.Id)
            .ToListAsync();
    }

    public async Task<List<Personel>> TumunuGorevleriyleGetirAsync()
    {
        return await _context.Personeller
            .Include(p => p.Gorevler)
            .OrderBy(p => p.Ad)
            .ThenBy(p => p.Soyad)
            .ToListAsync();
    }

    public async Task<Personel?> GetirAsync(int id)
    {
        return await _context.Personeller.FindAsync(id);
    }

    public async Task EkleAsync(Personel personel)
    {
        _context.Personeller.Add(personel);
        await _context.SaveChangesAsync();   // Değişikliği veritabanına yaz
    }

    public async Task GuncelleAsync(Personel personel)
    {
        _context.Personeller.Update(personel);
        await _context.SaveChangesAsync();
    }
}
