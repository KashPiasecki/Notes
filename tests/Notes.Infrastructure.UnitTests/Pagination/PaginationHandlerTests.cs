using Notes.Application.Common.Interfaces;
using Notes.Application.CQRS.Pagination;
using Notes.Domain.Contracts.Filters;
using Notes.Infrastructure.Pagination;
using TddXt.AnyRoot.Builder;
using TddXt.AnyRoot.Collections;
using TddXt.AnyRoot.Math;
using TddXt.AnyRoot.Network;
using TddXt.AnyRoot.Numbers;
using TddXt.AnyRoot.Strings;

namespace Notes.Infrastructure.UnitTests.Pagination;

public class PaginationHandlerTests
{
    [Test]
    public void ValidateQuery_ValidInput_ReturnsIt()
    {
        // Arrange
        var uriService = Any.Instance<IUriService>();
        
        var paginationHandler = new PaginationHandler(uriService);
        var paginationFilterQuery = Any.Instance<PaginationFilterQuery>()
            .WithProperty(x => x.PageNumber, Any.IntegerWithExactDigitsCount(1))
            .WithProperty(x => x.PageSize, Any.IntegerWithExactDigitsCount(1));

        // Act
        var result = paginationHandler.ValidateQuery(paginationFilterQuery);

        // Assert
        result.PageNumber.Should().Be(paginationFilterQuery.PageNumber);
        result.PageSize.Should().Be(paginationFilterQuery.PageSize);
    }
    
    [Test]
    public void ValidateQuery_InvalidInput_ReturnsDefaultValues()
    {
        // Arrange
        var uriService = Any.Instance<IUriService>();
        var paginationHandler = new PaginationHandler(uriService);
        var paginationFilterQuery = Any.Instance<PaginationFilterQuery>()
            .WithProperty(x => x.PageNumber, Any.Integer() * -1)
            .WithProperty(x => x.PageSize, Any.IntegerWithExactDigitsCount(10));

        // Act
        var result = paginationHandler.ValidateQuery(paginationFilterQuery);

        // Assert
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }
    
    [Test]
    public void CreatePagedResponse_CalledWithOnePageResult_ReturnsPagedResponse()
    {
        // Arrange
        var uriService = Substitute.For<IUriService>();
        var paginationHandler = new PaginationHandler(uriService);

        var tObject = Any.List<object>();
        var paginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 1)
            .WithProperty(x => x.PageSize, 10);
        var totalRecords = Any.IntegerWithExactDigitsCount(1);
        var route = Any.String();
        var validatedRoute = Any.Uri();
        uriService.GetPageUri(Arg.Any<PaginationFilter>(), route).Returns(validatedRoute);
        
        // Act
        var result = paginationHandler.CreatePagedResponse(tObject, paginationFilter, totalRecords, route);

        // Assert
        result.Data.Should().BeEquivalentTo(tObject);
        result.PageNumber.Should().Be(paginationFilter.PageNumber);
        result.PageSize.Should().Be(paginationFilter.PageSize);
        result.TotalPages.Should().Be(totalRecords / paginationFilter.PageSize +1);
        result.TotalRecords.Should().Be(totalRecords);
        result.NextPage.Should().Be(null);
        result.PreviousPage.Should().Be(null);
        result.FirstPage.Should().Be(validatedRoute);
        result.LastPage.Should().Be(validatedRoute);
    }
    
    [Test]
    public void CreatePagedResponse_CalledWithFewPagesResult_ReturnsPagedResponse()
    {
        // Arrange
        var uriService = Substitute.For<IUriService>();
        var paginationHandler = new PaginationHandler(uriService);

        var tObject = Any.List<object>();
        var paginationFilter = Any.Instance<PaginationFilter>()
            .WithProperty(x => x.PageNumber, 2)
            .WithProperty(x => x.PageSize, 10);
        var totalRecords = Any.IntegerWithExactDigitsCount(3);
        var route = Any.String();
        var validatedRoute = Any.Uri();
        uriService.GetPageUri(Arg.Any<PaginationFilter>(), route).Returns(validatedRoute);
        
        // Act
        var result = paginationHandler.CreatePagedResponse(tObject, paginationFilter, totalRecords, route);

        // Assert
        result.Data.Should().BeEquivalentTo(tObject);
        result.PageNumber.Should().Be(paginationFilter.PageNumber);
        result.PageSize.Should().Be(paginationFilter.PageSize);
        result.TotalPages.Should().Be(totalRecords / paginationFilter.PageSize +1);
        result.TotalRecords.Should().Be(totalRecords);
        result.NextPage.Should().Be(validatedRoute);
        result.PreviousPage.Should().Be(validatedRoute);
        result.FirstPage.Should().Be(validatedRoute);
        result.LastPage.Should().Be(validatedRoute);
    }
}