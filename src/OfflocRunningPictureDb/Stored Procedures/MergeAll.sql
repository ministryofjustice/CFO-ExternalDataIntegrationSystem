﻿CREATE PROCEDURE [OfflocRunningPicture].[MergeAll]
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
	BEGIN TRY;

		EXEC [OfflocRunningPicture].[MergeActivities]
		EXEC [OfflocRunningPicture].[MergeAddresses]
		EXEC [OfflocRunningPicture].[MergeAgencies]
		EXEC [OfflocRunningPicture].[MergeAssessments]
		EXEC [OfflocRunningPicture].[MergeBookings]
		EXEC [OfflocRunningPicture].[MergeEmployment]
		EXEC [OfflocRunningPicture].[MergeFlags]
		EXEC [OfflocRunningPicture].[MergeIdentifiers]
		EXEC [OfflocRunningPicture].[MergeIncentiveLevel]
		EXEC [OfflocRunningPicture].[MergeLocations]
		EXEC [OfflocRunningPicture].[MergeMainOffence]
		EXEC [OfflocRunningPicture].[MergeMovements]
		EXEC [OfflocRunningPicture].[MergeOffenderAgencies]
		EXEC [OfflocRunningPicture].[MergeOffenderStatus]
		EXEC [OfflocRunningPicture].[MergeOtherOffences]
		EXEC [OfflocRunningPicture].[MergePersonalDetails]
		EXEC [OfflocRunningPicture].[MergePNC]
		EXEC [OfflocRunningPicture].[MergePreviousPrisonNumbers]
		EXEC [OfflocRunningPicture].[MergeSentenceInformation]
		EXEC [OfflocRunningPicture].[MergeSexOffenders]

		--Non Temporal Table
		EXEC [OfflocRunningPicture].[MergeStandardisationReference]

		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ; THROW;
	END CATCH
END
