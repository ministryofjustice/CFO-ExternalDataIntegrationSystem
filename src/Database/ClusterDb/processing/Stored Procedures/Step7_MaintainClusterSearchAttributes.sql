
-- =============================================
-- Author:		Adam Bennett
-- Create date: 11/06/2024
-- Description: 
-- =============================================
CREATE PROCEDURE [processing].[Step7_MaintainClusterSearchAttributes]
AS
BEGIN

    SET NOCOUNT ON;

    --For each cluser maintain their attribute data for searching
    TRUNCATE TABLE search.ClusterAttributes;
    INSERT INTO search.ClusterAttributes
    SELECT c.ClusterId,
           c.UPCI2,
           cm.NodeName AS RecordSource,
           cm.NodeKey AS Identifier,
           CASE
               WHEN cm.NodeName = c.PrimaryRecordName
                    AND cm.NodeKey = c.PrimaryRecordKey THEN
                   1
               ELSE
                   0
           END AS PrimaryRecord,
           UPPER(   CASE
                        WHEN cm.NodeName = 'NOMIS' THEN
                            opd.Surname
                        WHEN cm.NodeName = 'DELIUS' THEN
                            do.Surname
                        ELSE
                            NULL
                    END
                ) AS LastName,
           CASE
               WHEN cm.NodeName = 'NOMIS' THEN
                   opd.DOB
               WHEN cm.NodeName = 'DELIUS' THEN
                   do.DateOfBirth
               ELSE
                   NULL
           END AS DOB
    FROM [output].Clusters c
        INNER JOIN [output].ClusterMembership cm
            ON c.ClusterId = cm.ClusterId
        LEFT OUTER JOIN [$(OfflocRunningPictureDb)].OfflocRunningPicture.PersonalDetails opd
            ON cm.NodeName = 'NOMIS'
               AND cm.NodeKey = opd.NOMSnumber
        LEFT OUTER JOIN [$(DeliusRunningPictureDb)].DeliusRunningPicture.Offenders do
            ON cm.NodeName = 'DELIUS'
               AND cm.NodeKey = do.CRN;




END;