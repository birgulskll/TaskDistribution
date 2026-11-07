using Microsoft.EntityFrameworkCore;
using TaskDistribution.Domain.Entities;

namespace TaskDistribution.Data.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly AppDbContext _context;

    public TaskItemRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> TumunuGetirAsync()
    {
        return await _context.Gorevler
            .Include(g => g.Islem)
            .Include(g => g.AtananPersonel)
            .OrderByDescending(g => g.OlusturmaTarihi)
            .ThenByDescending(g => g.Id)   // Aynı anda oluşan görevlerde sonraki kayıt üstte
            .ToListAsync();
    }

    public async Task EkleAsync(TaskItem gorev)
    {
        _context.Gorevler.Add(gorev);
        await _context.SaveChangesAsync();
    }
}
