using System.ComponentModel.DataAnnotations;
using API.DTOs.Offloc;
using Infrastructure.Entities.Offloc;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class InsertOrUpdateMainOffenceAction
{
    public static string Description = "Adds or Updates the sentence information for the given NOMIS number";

    internal static async Task<IResult> Action([FromServices] OfflocContext context, [Length(7, 7)] string nomsNumber,
        [FromBody] MainOffenceDto data)
    {
        var pd = await context.PersonalDetails
            .Include(p => p.MainOffences)
            .FirstOrDefaultAsync(r => r.NomsNumber == nomsNumber);

        if (pd == null)
        {
            return Results.NotFound("No record for for nomis number");
        }

        if (pd.MainOffences.Any() == false)
        {
            pd.MainOffences.Add(new MainOffence()
            {
                NomsNumber = pd.NomsNumber
            });
        }

        var mainOffence = pd.MainOffences.First();

        mainOffence.DateFirstConviction = data.DateFirstConviction;
        mainOffence.IsActive = data.IsActive;
        mainOffence.MainOffence1 = data.MainOffence;

        await context.SaveChangesAsync();

        return Results.Ok();

    }
}