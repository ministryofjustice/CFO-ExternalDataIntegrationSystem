using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace API.Endpoints.Actions;

public static class TransferCommunityAction
{
    public static string Description = "Updates the record's community record to use the new community code. This does not move them into community from prison if the community record is inactive.";

        internal static async Task<IResult> Action([FromServices] ClusteringContext clusterDb,
            [FromServices] DeliusContext deliusDb, [MaxLength(9), MinLength(9)] string upci,
            [MaxLength(3), MinLength(3)] string orgCode)
        {
            // get the cluster
            var cluster = await clusterDb.Clusters
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.UPCI == upci);

            if (cluster == null)
            {
                return Results.NotFound();
            }

            // work out the delius record
            var delius = cluster.Members.FirstOrDefault(c =>
                c.NodeName.Equals("Delius", StringComparison.CurrentCultureIgnoreCase));

            if (delius == null)
            {
                return Results.BadRequest("Given record does not have a delius record to move");
            }

            await deliusDb.Database.ExecuteSqlRawAsync(
                "UPDATE oomm SET oomm.OrgCode = {0} from DeliusRunningPicture.Offenders as o inner join DeliusRunningPicture.OffenderToOffenderManagerMappings oomm on o.OffenderId = oomm.OffenderId where crn = {1};",
                orgCode, delius.NodeKey);

   

        return Results.Ok($"Offender moved to {orgCode}");

        }

}
