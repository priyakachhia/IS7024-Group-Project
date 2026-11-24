using Cincy_Biz_Advisor.Models;
using Microsoft.EntityFrameworkCore;

namespace Cincy_Biz_Advisor.Data;

public class HistoryDbContext : DbContext
{
    public HistoryDbContext(DbContextOptions<HistoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<HistoryEntry> HistoryEntries => Set<HistoryEntry>();
}