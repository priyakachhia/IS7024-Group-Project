using Cincy_Biz_Advisor.Models;

namespace Cincy_Biz_Advisor.Services;

public interface IHistoryService
{
    Task<HistoryEntry> AddAsync(HistoryEntry entry, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<HistoryEntry>> GetAllAsync(CancellationToken cancellationToken = default);
}