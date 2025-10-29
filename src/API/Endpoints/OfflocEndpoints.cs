using API.DTOs.Offloc;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints;

public static class OfflocEndpoints
{
    public static IEndpointRouteBuilder RegisterOfflocEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/offloc")
            .WithTags("Offloc")
            .WithDisplayName("Offloc (Custody) Endpoints");

        group.MapGet("/{nomsNumber}/sentences", GetSentenceInformation)
            .Produces<SentenceDataDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.RequireAuthorization("read");

        return routes;
    }

    public static async Task<IResult> GetSentenceInformation([FromServices] OfflocContext offlocContext, string nomsNumber)
    {
        var query = from o in offlocContext.PersonalDetails
            where o.NomsNumber == nomsNumber
            select new SentenceDataDto()
            {
                NomsNumber = o.NomsNumber,
                SentenceInformation = o.SentenceInformation.Select(si => new SentenceInformationDto()
                {
                    IsActive = si.IsActive,
                    Crd = si.Crd,
                    DateOfRelease = si.DateOfRelease,
                    EarliestPossibleReleaseDate = si.EarliestPossibleReleaseDate,
                    FirstSentenced = si.FirstSentenced,
                    Hdcad = si.Hdcad,
                    Hdced = si.Hdced,
                    Led = si.Led,
                    Npd = si.Npd,
                    Ped = si.Ped,
                    Sed = si.Sed,
                    SentenceDays = si.SentenceDays,
                    SentenceMonths = si.SentenceMonths,
                    SentenceYears = si.SentenceYears,
                    Tused = si.Tused
                }).ToArray(),
                MainOffence = o.MainOffences.Select(mo => new MainOffenceDto()
                {
                    IsActive = mo.IsActive,
                    MainOffence = mo.MainOffence1,
                    DateFirstConviction = mo.DateFirstConviction
                }).ToArray(),
                OtherOffences = o.OtherOffences.Select(oo => new OtherOffenceDto()
                {
                    IsActive = oo.IsActive,
                    Details = oo.Details,
                }).ToArray()
            };
        
        var sentence = await query.FirstOrDefaultAsync();
        

        return sentence switch
        {
            null => Results.NotFound(""),
            _ => Results.Ok(sentence)
        };
    }
}
