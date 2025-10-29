using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class GetSearchCandidatesAction
{
    public static string Description = "Gets a list of search attributes";

    internal static async Task<IResult> Action([FromServices] ClusteringContext clusteringDb,
        int page = 1, int pageSize = 10)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Min(pageSize, 50);

        var result = await clusteringDb.Clusters
            .OrderBy(c => c.ClusterId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c =>
                new GetSearchCandidatesResultDto
                (
                    c.UPCI,
                    c.PrimaryRecordKey!,
                    c.PrimaryRecordName!,
                    c.ContainsInternalDupe,
                    c.ContainsLowProbabilityMembers,
                    c.RecordCount,
                    c.Attributes.Select(a => new AttributeDto(
                        a.DateOfBirth,
                        a.Identifier,
                        a.LastName,
                        a.PrimaryRecord,
                        a.RecordSource
                    )).ToArray()

                )).ToArrayAsync();
            
        return Results.Ok(result);
    }

    public record GetSearchCandidatesResultDto(
        string UPCI,
        string PrimaryRecordKey,
        string PrimaryRecordName,
        bool ContainsInternalDupe,
        bool ContainsLowProbabilityMembers,
        int RecordCount,
        AttributeDto[] Attributes
    );

    public record AttributeDto(
        DateOnly DateOfBirth,
        string Identifier,
        string LastName,
        bool PrimaryRecord,
        string RecordSource
    );

}