using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

public class IslemRepository : IIslemRepository
{
    private readonly AppDbContext _context;

    public IslemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Islem>> TumunuGetirAsync()
    {
        return await _context.Islemler
            .OrderBy(i => i.Zorluk)
            .ThenBy(i => i.Ad)
            .ToListAsync();
    }

    public async Task<Islem?> GetirAsync(int id)
    {
        return await _context.Islemler.FindAsync(id);
    }
}
