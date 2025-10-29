using API.Endpoints.Actions;
using static API.Endpoints.Actions.GetSearchCandidatesAction;

namespace API.Endpoints;

public static class DevelopmentEndpoints
{
    /// <summary>
    /// Registers the development endpoints, used for simulating moves / name changes ect.
    /// </summary>
    public static IEndpointRouteBuilder RegisterDevelopmentEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/development")
            .WithTags("Development")
            .WithDisplayName("Development and Testing Endpoints");

        group.MapPost("/{upci}/Release", ReleaseAction.Action)
            .WithDescription(ReleaseAction.Description)
            .ProducesProblem(404)
            .RequireAuthorization("write");

        group.MapPost("/{upci}/Incarcerate", IncarcerateAction.Action)
            .WithDescription(IncarcerateAction.Description)
            .RequireAuthorization("write");

        group.MapPost("/Prison/{upci}/Transfer/{establishmentCode}", TransferPrisonAction.Action)
            .WithDescription(TransferPrisonAction.Description)
            .RequireAuthorization("write");

        group.MapPost("/Community/{upci}/Transfer/{orgCode}", TransferCommunityAction.Action)
            .WithDescription(TransferCommunityAction.Description)
            .RequireAuthorization("write");

        group.MapGet("/Prison/{establishmentCode}/Offenders", OffendersByPrisonLocationAction.Action)
            .WithDescription(OffendersByPrisonLocationAction.Description)
            .Produces<OffenderByPrisonResult[]>();

        group.MapPost("/Prison/{NomsNumber}/SentenceInformation/", InsertOrUpdateSentenceInformationAction.Action)
            .WithDescription(InsertOrUpdateSentenceInformationAction.Description)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization("write");

        group.MapPost("/Prison/{NomsNumber}/MainOffence/", InsertOrUpdateMainOffenceAction.Action)
            .WithDescription(InsertOrUpdateMainOffenceAction.Description)
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization("write");

        group.MapGet("/clusters", GetSearchCandidatesAction.Action)
            .WithDescription(GetSearchCandidatesAction.Description)
            .Produces<GetSearchCandidatesResultDto[]>();

        group.RequireAuthorization("read");

        return routes;
    }

    
}