CREATE PROCEDURE [OfflocStaging].[ClearOfflocStaging]
AS
BEGIN
	TRUNCATE TABLE [OfflocStaging].Activities;
	TRUNCATE TABLE [OfflocStaging].Addresses;
	TRUNCATE TABLE [OfflocStaging].Agencies;
	TRUNCATE TABLE [OfflocStaging].Assessments;
	TRUNCATE TABLE [OfflocStaging].Bookings;
	TRUNCATE TABLE [OfflocStaging].Employment;
	TRUNCATE TABLE [OfflocStaging].Flags;
	TRUNCATE TABLE [OfflocStaging].Identifiers;
	TRUNCATE TABLE [OfflocStaging].IncentiveLevel;
	TRUNCATE TABLE [OfflocStaging].Locations;
	TRUNCATE TABLE [OfflocStaging].MainOffence;
	TRUNCATE TABLE [OfflocStaging].Movements;
	TRUNCATE TABLE [OfflocStaging].OffenderAgencies;
	TRUNCATE TABLE [OfflocStaging].OffenderStatus;
	TRUNCATE TABLE [OfflocStaging].OtherOffences;
	TRUNCATE TABLE [OfflocStaging].PersonalDetails;
	TRUNCATE TABLE [OfflocStaging].PNC;
	TRUNCATE TABLE [OfflocStaging].PreviousPrisonNumbers;
	TRUNCATE TABLE [OfflocStaging].SentenceInformation;
	TRUNCATE TABLE [OfflocStaging].SexOffenders;
	TRUNCATE TABLE [OfflocStaging].VeteranFlagLog;
	TRUNCATE TABLE [OfflocStaging].StandardisationReference;
END
