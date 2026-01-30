using API.Services;
using Infrastructure.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class VisualisationEndpoints
{
    public static IEndpointRouteBuilder RegisterVisualisationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/visualisation")
            .WithTags("Visualisation")
            .WithDisplayName("Visualisation Endpoints");

        group.MapGet("/{upci}/Details", GetDetailsByUpciAsync)
            .Produces<ClusterDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/Generate", GenerateClusterAsync)
            .Produces<ClusterDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/Save", SaveNetworkAsync)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("visualisation-write");


        group.MapGet("/processedfiles", GetRecentProcessedFiles)
            .Produces<ProcessedFileDto[]>();

        group.RequireAuthorization("visualisation-read");

        return routes;
    }

    private static async Task<IResult> GetRecentProcessedFiles([FromServices] OfflocContext offlocContext, [FromServices] DeliusContext deliusContext)
    {
        var offlocFiles = await offlocContext.ProcessedFiles
                            .OrderByDescending(pf => pf.ValidFrom)
                            .Take(5)
                            .Select(c => new ProcessedFileDto()
                            {
                                FileName = c.FileName,
                                Status = c.Status,
                                ValidFrom = c.ValidFrom
                            }).ToArrayAsync();

        var deliusFiles = await deliusContext.ProcessedFiles
                            .OrderByDescending(pf => pf.ValidFrom)
                            .Take(5)
                            .Select(c => new ProcessedFileDto()
                            {
                                FileName = c.FileName,
                                Status = c.Status,
                                ValidFrom = c.ValidFrom
                            }).ToArrayAsync();

        return Results.Ok(offlocFiles.Union(deliusFiles));
    }

    public static async Task<IResult> GetDetailsByUpciAsync([FromServices] ApiServices services, string upci)
    {
        var cluster = await services.VisualisationRepository.GetDetailsByUpciAsync(upci);
        return cluster is null
            ? Results.NotFound()
            : Results.Ok(cluster);
    }

    public static async Task<IResult> GenerateClusterAsync([FromServices] ApiServices services)
    {
        var cluster = await services.ClusteringRepository.GenerateClusterAsync();

        if (cluster is null)
        {
            return Results.NotFound();
        }

        return Results.Ok(ClusterDto.Empty(cluster.UPCI));
    }

    public static async Task<IResult> SaveNetworkAsync([FromServices] ApiServices services, [FromBody] NetworkDto network)
    {
        var success = await services.VisualisationRepository.SaveNetworkAsync(network);

        return success is false
            ? Results.InternalServerError()
            : Results.Ok();

    }

}
