using Cincy_Biz_Advisor.Models;

namespace Cincy_Biz_Advisor.Services;

public interface INeighborhoodRepository
{
    IReadOnlyList<Neighborhood> GetAvailableNeighborhoods();

    Neighborhood? GetById(int id);
}
