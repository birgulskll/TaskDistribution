using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

/// <summary>
/// IPersonelRepository'nin EF Core uygulaması.
/// </summary>
public class PersonelRepository : IPersonelRepository
{
    private readonly AppDbContext _context;

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
            .Include(p => p.Gorevler)
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
        await _context.SaveChangesAsync();
    }

    public async Task GuncelleAsync(Personel personel)
    {
        _context.Personeller.Update(personel);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> GoreviVarMiAsync(int personelId)
    {
        return await _context.Gorevler.AnyAsync(g => g.AtananPersonelId == personelId);
    }

    public async Task SilAsync(Personel personel)
    {
        _context.Personeller.Remove(personel);
        await _context.SaveChangesAsync();
    }
}
