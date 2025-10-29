using API.Services;
using Infrastructure.DTOs;
using Infrastructure.Entities.Aggregation;
using Infrastructure.Entities.Clustering;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class ClusteringEndpoints
{
    public static IEndpointRouteBuilder RegisterClusteringEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/clustering")
            .WithTags("Clustering")
            .WithDisplayName("Clustering Endpoints");

        group.MapGet("/{clusterId:int}", GetByIdAsync)
            .Produces<Cluster>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{upci}", GetByUpciAsync)
            .Produces<Cluster>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{upci}/Aggregate", GetAggregateByUpciAsync)
            .Produces<ClusterAggregate>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/Generate", GenerateClusterAsync)
            .Produces<Cluster>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.RequireAuthorization("read");

        return routes;
    }

    public static async Task<IResult> GetByIdAsync([FromServices] ApiServices services, int clusterId)
    {
        var cluster = await services.ClusteringRepository.GetByIdAsync(clusterId);
        return cluster is null
            ? Results.NotFound()
            : Results.Ok(cluster);
    }

    public static async Task<IResult> GetByUpciAsync([FromServices] ApiServices services, string upci)
    {
        var cluster = await services.ClusteringRepository.GetByUpciAsync(upci);
        return cluster is null
            ? Results.NotFound()
            : Results.Ok(cluster);
    }

    public static async Task<IResult> GetAggregateByUpciAsync([FromServices] ApiServices services, string upci)
    {
        var aggregate = await services.AggregateService.GetClusterAggregateAsync(upci);
        return aggregate is null
            ? Results.NotFound()
            : Results.Ok(aggregate);
    }

    public static async Task<IResult> GenerateClusterAsync([FromServices] ApiServices services)
    {
        var cluster = await services.ClusteringRepository.GenerateClusterAsync();

        return cluster is null
            ? Results.NotFound()
            : Results.Ok(cluster);
    }

}
