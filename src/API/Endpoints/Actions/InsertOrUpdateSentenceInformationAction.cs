using System.ComponentModel.DataAnnotations;
using API.DTOs.Offloc;
using Infrastructure.Entities.Offloc;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class InsertOrUpdateSentenceInformationAction
{
    public static string Description = "Adds or Updates the sentence information for the given NOMIS number";

    internal static async Task<IResult> Action([FromServices] OfflocContext context, [Length(7, 7)] string nomsNumber,
        [FromBody] SentenceInformationDto data)
    {

        var pd = await context.PersonalDetails
            .Include(p => p.SentenceInformation)
            .FirstOrDefaultAsync(r => r.NomsNumber == nomsNumber);

        if (pd == null)
        {
            return Results.NotFound("No record for for nomis number");
        }

        if (pd.SentenceInformation.Any() == false)
        {
            pd.SentenceInformation.Add(new SentenceInformation()
            {
                NomsNumber = pd.NomsNumber
            });
        }

        var sentence = pd.SentenceInformation.First();

        sentence.Crd = data.Crd;
        sentence.DateOfRelease = data.DateOfRelease;
        sentence.EarliestPossibleReleaseDate = data.EarliestPossibleReleaseDate;
        sentence.FirstSentenced = data.FirstSentenced;
        sentence.Hdcad = data.Hdcad;
        sentence.Hdced = data.Hdced;
        sentence.IsActive = data.IsActive;
        sentence.Led = data.Led;
        sentence.NomsNumber = nomsNumber;
        sentence.Npd = data.Npd;
        sentence.Ped = data.Ped;
        sentence.Sed = data.Sed;
        sentence.SentenceDays = data.SentenceDays;
        sentence.SentenceMonths = data.SentenceMonths;
        sentence.SentenceYears = data.SentenceYears;
        sentence.Tused = data.Tused;

        await context.SaveChangesAsync();

        return Results.Ok();
    }


}