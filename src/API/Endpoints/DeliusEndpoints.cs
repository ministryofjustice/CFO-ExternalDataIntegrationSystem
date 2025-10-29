using API.DTOs.Delius;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Endpoints;

public static class DeliusEndpoints
{
    public static IEndpointRouteBuilder RegisterDeliusEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/delius")
            .WithTags("Delius")
            .WithDisplayName("Delius (Community) Endpoints");

        group.MapGet("/{crn}/offences/", GetOffenceDetailsByCrnAsync)
            .WithDescription("Returns an OffenderDto with offence, disposal and requirement information")
            .Produces<OffenceDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{crn}/offendermanagersummary/", GetOffenderManagerSummaryByCrnAsync)
            .WithDescription("Returns an OffenderManagerSummaryDto with Organisation and Team information")
            .Produces<OffenderManagerSummaryDto>()
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.RequireAuthorization("read");

        return routes;
    }

    public static async Task<IResult> GetOffenceDetailsByCrnAsync([FromServices] DeliusContext context, string crn)
    {
        var query = from o in context.Offenders
            where o.Crn == crn
            select new OffenceDto
            {
                Crn = o.Crn,
                IsDeleted = o.Deleted == "Y",
                MainOffences = (
                    from mo in context.MainOffences
                    where
                        mo.OffenderId == o.Id
                        && mo.Deleted == "N" //TODO: Allow an Admin role to exclude this check
                    orderby mo.OffenceDate descending
                    select new MainOffenceDto()
                    {
                        OffenceDescription = mo.OffenceDescription,
                        OffenceDate = mo.OffenceDate,
                        IsDeleted = mo.Deleted == "Y",
                        Disposals = (
                            from d in context.Disposals
                            where d.EventId == mo.EventId && d.Deleted == "N" //TODO: Allow an Admin role to exclude this check
                            select new DisposalDto()
                            {
                                SentenceDate = d.SentenceDate,
                                Length = d.Length,
                                UnitDescription = d.UnitDescription,
                                DisposalDetail = d.DisposalDetail,
                                TerminationDescription = d.DisposalTerminationDescription,
                                TerminationDate = d.TerminationDate,
                                IsDeleted = d.Deleted == "Y",
                                Requirements = (
                                    from r in context.Requirements
                                    where r.DisposalId == d.Id && r.Deleted == "N" //TODO: Allow an Admin role to exclude this check
                                    select new RequirementDto()
                                    {
                                        CategoryDescription = r.CategoryDescription,
                                        SubCategoryDescription = r.SubCategoryDescription,
                                        TerminationDescription = r.TerminationDescription,
                                        Length = r.Length,
                                        UnitDescription = r.UnitDescription,
                                        IsDeleted = r.Deleted == "Y"
                                    }
                                ).ToArray()
                            }
                        ).ToArray()
                    }
                ).ToArray()
            };

        var offender = await query.FirstOrDefaultAsync();

        return offender is null
            ? Results.NotFound()
            : Results.Ok(offender);

    }
    public static async Task<IResult> GetOffenderManagerSummaryByCrnAsync([FromServices] DeliusContext context, string crn)
    {
        var query = from otomm in context.OffenderToOffenderManagerMappings
                    join o in context.Offenders on otomm.OffenderId equals o.OffenderId
                    join omt in context.OffenderManagerTeams on new { otomm.OrgCode, otomm.TeamCode } equals new { omt.OrgCode, omt.TeamCode }
                    where otomm.EndDate == null
                       && otomm.Deleted != "Y"
                       && o.Crn == crn
                    select new OffenderManagerSummaryDto
                    {
                        OrganisationCode = omt.OrgCode,
                        TeamCode = omt.TeamCode,
                        OrganistationDescription = omt.OrgDescription,
                        TeamDescription = omt.TeamDescription
                    };


        OffenderManagerSummaryDto? offenderManagerSummary = await query.FirstOrDefaultAsync();

        return offenderManagerSummary is null
            ? Results.NotFound()
            : Results.Ok(offenderManagerSummary);
    }
}
