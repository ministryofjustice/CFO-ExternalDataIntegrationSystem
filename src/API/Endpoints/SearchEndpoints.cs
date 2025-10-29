using API.Services;
using Matching.Core.Search;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class SearchEndpoints
{
    public static IEndpointRouteBuilder RegisterSearchEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/search")
            .WithTags("Search")
            .WithDisplayName("Search Endpoints");
            
        group.MapGet("/", SearchAsync)
            .Produces<SearchResult[]>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.RequireAuthorization("read");

        return routes;
    }

    public static async Task<IResult> SearchAsync(
        [FromServices] ApiServices services,
        string identifier,
        string lastName,
        DateOnly dateOfBirth)
    {
        var records = await services.ClusteringRepository
            .SearchAsync(identifier, lastName, dateOfBirth);

        return records switch
        {
            [] => Results.NotFound(),
            _ => Results.Ok(records.Select
                (
                    record => new SearchResult
                    (
                        Upci: record.UPCI,
                        Precedence: Precedence.GetPrecedence
                        (
                            (identifier, record.Identifier),
                            (lastName, record.LastName),
                            (dateOfBirth, record.DateOfBirth)
                        )
                    )
                )
                .GroupBy(result => result.Upci).Select
                (
                    result => new SearchResult
                    (
                        Upci: result.Key,
                        Precedence: result.Min(r => r.Precedence)
                    )
                )
                .OrderBy(r => r.Precedence))
        };
    }
}

public record SearchResult(string Upci, int Precedence);
