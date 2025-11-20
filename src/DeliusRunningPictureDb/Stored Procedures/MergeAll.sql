CREATE PROCEDURE [DeliusRunningPicture].[MergeAll]
	@fileName NVARCHAR(255) NULL
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
	BEGIN TRY;
		
		--In theory not executing the pre-merge scripts should fulfill the soft delete Carl wants.
		
		EXEC [DeliusRunningPicture].[MergeAdditionalIdentifier];
		EXEC [DeliusRunningPicture].[MergeAliasDetails];
		EXEC [DeliusRunningPicture].[MergeDisability];
		EXEC [DeliusRunningPicture].[MergeDisposal];
		EXEC [DeliusRunningPicture].[MergeEventDetails];
		EXEC [DeliusRunningPicture].[MergeMainOffence];
		EXEC [DeliusRunningPicture].[MergeOAS];
		EXEC [DeliusRunningPicture].[MergeOffenderAddress];
		EXEC [DeliusRunningPicture].[MergeOffenderManager];
		EXEC [DeliusRunningPicture].[MergeOffenderManagerBuildings];
		EXEC [DeliusRunningPicture].[MergeOffenderManagerTeam];
		EXEC [DeliusRunningPicture].[MergeOffenders];
		EXEC [DeliusRunningPicture].[MergeOffenderToOffenderManagerMappings];
		EXEC [DeliusRunningPicture].[MergeOffenderTransfer];
		EXEC [DeliusRunningPicture].[MergePersonalCircumstances];
		EXEC [DeliusRunningPicture].[MergeProvision];
		EXEC [DeliusRunningPicture].[MergeRegistrationDetails];
		EXEC [DeliusRunningPicture].[MergeRequirement];

		--Non Temporal Table
		EXEC [DeliusRunningPicture].[MergeStandardisationReference]

		IF(@fileName IS NOT NULL)
		BEGIN
			UPDATE [DeliusRunningPicture].[ProcessedFiles]
			SET [Status] = 'Merged'
			WHERE [FileName] = @fileName;
		END;

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ; THROW;
	END CATCH;

END
