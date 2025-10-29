using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class OffendersByPrisonLocationAction
{
    public static string Description => "Gets offender details for the given establishment.";

    internal static async Task<IResult> Action([FromServices] OfflocContext offlocDb, [MaxLength(3), MinLength(3)]string establishmentCode)
    {
        try
        {
            var query = from pd in offlocDb.PersonalDetails
                join oa in offlocDb.OffenderAgencies
                    on pd.NomsNumber equals oa.NomsNumber
                where oa.EstablishmentCode == establishmentCode
                select new OffenderByPrisonResult
                    {
                    FirstName =  pd.FirstName,
                    Surname = pd.Surname,
                    DateOfBirth = pd.DateOfBirth,
                    IsPrimaryRecord = pd.IsActive,
                    EstablishmentCode = oa.EstablishmentCode,
                    IsActiveInLocation = oa.IsActive
                    };

            var results = await query.ToArrayAsync();

            return Results.Ok(results);
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

}

public class OffenderByPrisonResult
{
    public required string FirstName { get; set; }
    public required string Surname { get; set; }
    public required DateOnly DateOfBirth { get; set; }
    public required bool IsPrimaryRecord { get; set; }
    public required string EstablishmentCode { get; set; }
    public required bool IsActiveInLocation { get; set; }


}