using System.ComponentModel.DataAnnotations;
using Infrastructure.Entities.Clustering;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class SetStickyLocationAction
{
    public static string Description = "Sets a records sticky community location this will override the delius org code";

    internal static async Task<IResult> Action([FromServices] ClusteringContext clusterDb, [Length(9,9)]string upci, [Length(4,4)]string orgCode)
    {
        try
        {
            var entry = await clusterDb.StickyLocations
                .Where(e => e.Upci == upci)
                .FirstOrDefaultAsync();

            if (entry == null)
            {
                entry = new StickyLocation()
                {
                    Upci = upci,
                    OrgCode = orgCode
                };
                clusterDb.StickyLocations.Add(entry);
            }
            else
            {
                entry.OrgCode = orgCode;
            }

            await clusterDb.SaveChangesAsync();

            return Results.Accepted();
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

}