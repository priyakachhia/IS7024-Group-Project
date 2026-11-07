using Cincy_Biz_Advisor.Models;

namespace Cincy_Biz_Advisor.Services;

public sealed class InMemoryNeighborhoodRepository : INeighborhoodRepository
{
    private static readonly IReadOnlyList<Neighborhood> Neighborhoods =
    [
        new Neighborhood { Id = 1, Name = "Over-the-Rhine" },
        new Neighborhood { Id = 2, Name = "Oakley" }
    ];

    public IReadOnlyList<Neighborhood> GetAvailableNeighborhoods()
    {
        return Neighborhoods;
    }

    public Neighborhood? GetById(int id)
    {
        return Neighborhoods.SingleOrDefault(neighborhood => neighborhood.Id == id);
    }
}
