CREATE PROCEDURE [OfflocStaging].[StandardiseData] @retMessage varchar(200) OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result SETs FROM
	-- interfering WITH SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRANSACTION;	
	BEGIN TRY
		Declare @retVal int = 0;
		
		EXEC [OfflocStaging].[InitializeStandardisationLookups];

		/***************************************************Offloc Standardisation START**************************************/

		--Postcode
		EXEC [OfflocStaging].[StandardisePostCode] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Offloc Staging StandardisePostCode';
		END

		--Phone Number
		EXEC [OfflocStaging].[StandardisePhoneNumber] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Offloc Staging StandardisePhoneNumber';
		END

		--Prison Number 
		EXEC [OfflocStaging].[StandardisePrisonNumber] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Offloc Staging StandardisePrisonNumber';
		END

		--CRO
		EXEC [OfflocStaging].[StandardiseCRO] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Offloc Staging StandardiseCRO';
		END

		--PNC
		EXEC [OfflocStaging].[StandardisePNC] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Offloc Staging StandardisePNC';
		END


		/***************************************************Offloc Standardisation END****************************************/
		

		/***************************************************Offloc Add Reference Data START****************************************/
		EXEC [OfflocStaging].[AddReferenceData] @retVal;
		IF(@retVal < 0)
		BEGIN
			set @retMessage = 'Something went wrong  in Offloc Staging AddReferenceData';
		END
		/***************************************************Offloc Add Reference Data END******************************************/

		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END
