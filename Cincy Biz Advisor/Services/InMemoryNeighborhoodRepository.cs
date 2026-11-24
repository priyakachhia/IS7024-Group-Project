using Cincy_Biz_Advisor.Models;

namespace Cincy_Biz_Advisor.Services;

public sealed class InMemoryNeighborhoodRepository : INeighborhoodRepository
{
    private static readonly IReadOnlyList<Neighborhood> Neighborhoods =
        CincinnatiNeighborhoods.All
            .Select((neighborhood, index) => neighborhood with
            {
                Id = neighborhood.DisplayName switch
                {
                    "Over-the-Rhine" => 1,
                    "Oakley" => 2,
                    _ => index + 3
                }
            })
            .ToArray();

    public IReadOnlyList<Neighborhood> GetAvailableNeighborhoods()
    {
        return Neighborhoods;
    }

    public Neighborhood? GetById(int id)
    {
        return Neighborhoods.SingleOrDefault(neighborhood => neighborhood.Id == id);
    }
}