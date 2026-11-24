using Cincy_Biz_Advisor.Data;
using Cincy_Biz_Advisor.Models;
using Microsoft.EntityFrameworkCore;

namespace Cincy_Biz_Advisor.Services;

public class HistoryService : IHistoryService
{
    private readonly HistoryDbContext _db;

    public HistoryService(HistoryDbContext db)
    {
        _db = db;
    }

    public async Task<HistoryEntry> AddAsync(HistoryEntry entry, CancellationToken cancellationToken = default)
    {
        entry.SavedAtUtc = DateTime.UtcNow;
        _db.HistoryEntries.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<IReadOnlyList<HistoryEntry>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entries = await _db.HistoryEntries
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return entries
            .OrderByDescending(e => e.SavedAtUtc)
            .ThenByDescending(e => e.Id)
            .ToList();
    }
}