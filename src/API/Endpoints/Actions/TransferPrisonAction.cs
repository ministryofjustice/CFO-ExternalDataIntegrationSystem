using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class TransferPrisonAction
{
    public static string Description = "Updates the record's custody record to use the new prison code. This does not move them into prison from community if the prison record is inactive.";

    internal static async Task<IResult> Action([FromServices] ClusteringContext clusterDb,
        [FromServices] OfflocContext offlocDb, [MaxLength(9), MinLength(9)] string upci,
        [MaxLength(3), MinLength(3)] string establishmentCode)
    {
        // get the cluster
        var cluster = await clusterDb.Clusters
            .Include(x => x.Members)
            .FirstOrDefaultAsync(x => x.UPCI == upci);

        if (cluster == null)
        {
            return Results.NotFound();
        }

        // work out the nomis record
        var nomis = cluster.Members.FirstOrDefault(c =>
            c.NodeName.Equals("NOMIS", StringComparison.CurrentCultureIgnoreCase));

        if (nomis == null)
        {
            return Results.BadRequest("Given record does not have a nomis record to move");
        }

        await offlocDb.Database.ExecuteSqlRawAsync(
            "UPDATE [OfflocRunningPicture].[OffenderAgencies] SET EstablishmentCode = {0}, IsActive = 1 WHERE NomsNumber = {1}",
            establishmentCode, nomis.NodeKey);

   

        return Results.Ok($"Offender moved to {establishmentCode}");
    }
}