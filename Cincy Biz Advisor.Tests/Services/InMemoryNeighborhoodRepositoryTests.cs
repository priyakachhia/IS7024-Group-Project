using Cincy_Biz_Advisor.Services;

namespace Cincy_Biz_Advisor.Tests.Services;

public sealed class InMemoryNeighborhoodRepositoryTests
{
    private readonly INeighborhoodRepository _repository = new InMemoryNeighborhoodRepository();

    [Fact]
    public void SeededNeighborhoodsIncludeRequiredRecords()
    {
        var neighborhoods = _repository.GetAvailableNeighborhoods();

        Assert.Contains(neighborhoods, neighborhood => neighborhood.Name == "Over-the-Rhine");
        Assert.Contains(neighborhoods, neighborhood => neighborhood.Name == "Oakley");
    }

    [Fact]
    public void SeededNeighborhoodsHavePositiveUniqueIdsAndNames()
    {
        var neighborhoods = _repository.GetAvailableNeighborhoods();

        Assert.All(neighborhoods, neighborhood => Assert.True(neighborhood.Id > 0));
        Assert.Equal(neighborhoods.Count, neighborhoods.Select(neighborhood => neighborhood.Id).Distinct().Count());
        Assert.Equal(neighborhoods.Count, neighborhoods.Select(neighborhood => neighborhood.Name).Distinct(StringComparer.Ordinal).Count());
    }

    [Fact]
    public void LookupByIdReturnsTheMatchingNeighborhood()
    {
        var expected = _repository.GetAvailableNeighborhoods().Single(neighborhood => neighborhood.Name == "Oakley");

        var actual = _repository.GetById(expected.Id);

        Assert.NotNull(actual);
        Assert.Equal(expected.Id, actual.Id);
        Assert.Equal(expected.Name, actual.Name);
    }

    [Fact]
    public void LookupByUnknownIdReturnsNoNeighborhood()
    {
        Assert.Null(_repository.GetById(int.MaxValue));
    }

    [Fact]
    public void AvailableNeighborhoodsExposeTheRepositoryCollectionContract()
    {
        var neighborhoods = _repository.GetAvailableNeighborhoods();

        Assert.NotNull(neighborhoods);
        Assert.NotEmpty(neighborhoods);
    }
}
