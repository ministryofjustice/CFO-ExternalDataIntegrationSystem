using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class ReleaseAction
{

    public static string Description => "Marks the community record as active, simulating a release from prison.";
                                        

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

            cluster.PrimaryRecordKey = delius.NodeKey;
            cluster.PrimaryRecordName = "DELIUS";

            await clusterDb.SaveChangesAsync();

            await offlocDb.PersonalDetails
                .Where(p => p.NomsNumber == offloc.NodeKey)
                .ExecuteUpdateAsync(p => p.SetProperty(n => n.IsActive, false));

            await deliusDb.Offenders
                .Where(p => p.Crn == delius.NodeKey)
                .ExecuteUpdateAsync(p => p.SetProperty(n => n.Deleted, "N"));

            return Results.Ok("Record moved to community");
        }
        catch (Exception ex)
        {
            return Results.BadRequest(ex.Message);
        }



    }
}