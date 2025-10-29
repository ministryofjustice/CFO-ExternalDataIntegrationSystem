/*Deletes records that we know are now not on Staging */
CREATE PROCEDURE [DeliusRunningPicture].[PreMergeScript]
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;

    BEGIN TRY

        DECLARE @listOfOffenderIdsToBeRemoved TABLE
        (
            OffenderId INT
        );

        INSERT INTO @listOfOffenderIdsToBeRemoved
        SELECT [OffenderId]
        FROM [DeliusStagingDb].[DeliusStaging].[Disposal]
        WHERE [Deleted] = 'Y' OR TerminationDate IS NOT NULL 
        GROUP BY [OffenderId];

        INSERT INTO @listOfOffenderIdsToBeRemoved
        SELECT [OffenderId] FROM [DeliusStagingDb].[DeliusStaging].[Offenders] WHERE [Deleted] = 'Y'
        AND OffenderId NOT IN (SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved);

        --Section below handles direct deletion of running picture.
        DELETE FROM [DeliusRunningPicture].[AdditionalIdentifier] WHERE Id IN 
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.AdditionalIdentifier WHERE Deleted = 'Y') ;
        DELETE FROM [DeliusRunningPicture].[AliasDetails] WHERE Id IN 
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.AliasDetails WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[Disability] WHERE Id IN 
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.Disability WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[Disposal] WHERE Id IN 
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.Disposal WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[EventDetails] WHERE Id IN 
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.EventDetails WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[MainOffence] WHERE Id IN 
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.MainOffence WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[OAS] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.OAS WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[OffenderAddress] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.OffenderAddress WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[Offenders] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.Offenders WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[OffenderToOffenderManagerMappings] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.OffenderToOffenderManagerMappings WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[OffenderTransfer] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.OffenderTransfer WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[PersonalCircumstances] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.PersonalCircumstances WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[Provision] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.Provision WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[RegistrationDetails] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.RegistrationDetails WHERE Deleted = 'Y');
        DELETE FROM [DeliusRunningPicture].[Requirement] WHERE Id IN
        (SELECT Id FROM DeliusStagingDb.DeliusStaging.Requirement WHERE Deleted = 'Y');

        DELETE FROM DeliusStagingDb.DeliusStaging.AdditionalIdentifier WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.AliasDetails WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.Disability WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.Disposal WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.EventDetails WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.MainOffence WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.OAS WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.OffenderAddress WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.Offenders WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.OffenderToOffenderManagerMappings WHERE Deleted = 'Y';        
        DELETE FROM DeliusStagingDb.DeliusStaging.OffenderTransfer WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.PersonalCircumstances WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.Provision WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.RegistrationDetails WHERE Deleted = 'Y';
        DELETE FROM DeliusStagingDb.DeliusStaging.Requirement WHERE Deleted = 'Y';

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[AdditionalIdentifier]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[AliasDetails]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[Disability]
        WHERE OffenderId IN 
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[Disposal]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[EventDetails]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[Header]
        WHERE OffenderId IN 
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[MainOffence]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OAS]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OffenderAddress]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[Offenders]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OffenderTransfer]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[PersonalCircumstances]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[Provision]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[RegistrationDetails]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[Requirement]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        -- Ordering for remaining deletions is important.
        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OffenderToOffenderManagerMappings] 
        WHERE OffenderId IN(
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );        

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OffenderManager]
        WHERE OmCode NOT IN (Select OmCode FROM [DeliusStagingDb].[DeliusStaging].[OffenderToOffenderManagerMappings]);

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OffenderManagerTeam]
        WHERE TeamCode NOT IN (Select TeamCode FROM [DeliusStagingDb].[DeliusStaging].[OffenderManager]);

        DELETE FROM [DeliusStagingDb].[DeliusStaging].[OffenderManagerBuildings]
        WHERE CompositeHash NOT IN (Select CompositeBuildingHash FROM [DeliusStagingDb].[DeliusStaging].[OffenderManagerTeam]);

        --Below sections handle running picture.
        DELETE FROM [DeliusRunningPicture].[AdditionalIdentifier]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[AliasDetails]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[Disability]
        WHERE OffenderId IN 
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[Disposal]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[EventDetails]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[MainOffence]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[OAS]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[OffenderAddress]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[Offenders]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[OffenderTransfer]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[PersonalCircumstances]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[Provision]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[RegistrationDetails]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        DELETE FROM [DeliusRunningPicture].[Requirement]
        WHERE OffenderId IN
        (
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );

        -- Ordering for remaining deletions is important.
        DELETE FROM [DeliusRunningPicture].[OffenderToOffenderManagerMappings] 
        WHERE OffenderId IN(
            SELECT OffenderId FROM @listOfOffenderIdsToBeRemoved
        );        

        DELETE FROM [DeliusRunningPicture].[OffenderManager]
        WHERE OmCode NOT IN (Select OmCode FROM [DeliusRunningPicture].[OffenderToOffenderManagerMappings]);

        DELETE FROM [DeliusRunningPicture].[OffenderManagerTeam]
        WHERE TeamCode NOT IN (Select TeamCode FROM [DeliusRunningPicture].[OffenderManager]);

        DELETE FROM [DeliusRunningPicture].[OffenderManagerBuildings]
        WHERE CompositeHash NOT IN (Select CompositeBuildingHash FROM [DeliusRunningPicture].[OffenderManagerTeam]);

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
        BEGIN
            ROLLBACK TRANSACTION;
        END;
        ; THROW;
    END CATCH;
END;
