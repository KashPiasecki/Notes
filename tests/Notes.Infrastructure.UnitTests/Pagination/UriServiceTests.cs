using Notes.Domain.Contracts.Filters;
using Notes.Infrastructure.Pagination;
using TddXt.AnyRoot.Network;
using TddXt.AnyRoot.Strings;

namespace Notes.Infrastructure.UnitTests.Pagination;

public class UriServiceTests
{
    [Test]
    public void GetPageUri_Called_ConcatenatesPagination()
    {
        // Arrange
        var uri = Any.Uri().ToString();
        
        var uriService = new UriService(uri);
        var paginationFilter = Any.Instance<PaginationFilter>();
        var route = Any.String();

        // Act
        var result = uriService.GetPageUri(paginationFilter, route);

        // Assert
        result.Should().Be($"{uri}{route}?pageNumber={paginationFilter.PageNumber}&pageSize={paginationFilter.PageSize}");
    }
}