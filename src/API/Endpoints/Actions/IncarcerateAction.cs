using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class IncarcerateAction
{
    public static string Description = "Marks the custody record as active, simulating a return to prison.";

    internal static async Task<IResult> Action([FromServices] ClusteringContext clusterDb, [FromServices] OfflocContext offlocDb, DeliusContext deliusDb, string upci)
    {
        try
        {
            // get the cluster records for this UPCI
            var cluster = await clusterDb.Clusters
                .Include(c => c.Members)
                .Where(e => e.UPCI == upci)
                .FirstOrDefaultAsync();

            if (cluster == null)
            {
                return Results.NotFound();
            }

            var delius = cluster.Members.FirstOrDefault(c => c.NodeName == "DELIUS");
            var offloc = cluster.Members.FirstOrDefault(c => c.NodeName == "NOMIS");

            if (delius == null || offloc == null)
            {
                return Results.BadRequest("Need at least 1 record from both nomis and delius to release");
            }

            cluster.PrimaryRecordKey = offloc.NodeKey;
            cluster.PrimaryRecordName = "NOMIS";

            await clusterDb.SaveChangesAsync();

            await offlocDb.PersonalDetails
                .Where(p => p.NomsNumber == offloc.NodeKey)
                .ExecuteUpdateAsync(p => p.SetProperty(n => n.IsActive, true));

            await deliusDb.Offenders
                .Where(p => p.Crn == delius.NodeKey)
                .ExecuteUpdateAsync(p => p.SetProperty(n => n.Deleted, "Y"));

            return Results.Ok("Record moved to custody");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }

    }

}