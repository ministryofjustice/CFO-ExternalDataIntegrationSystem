CREATE OR ALTER PROCEDURE [dbo].[CreateDummyCandidate](
	@Identifier NVARCHAR(9), 
	@FirstName NVARCHAR(50), 
	@LastName NVARCHAR(50), 
	@DateOfBirth VARCHAR(50), 
	@NomisNumber NVARCHAR(10),
	@Crn NVARCHAR(10),
	@IsActive BIT=1,
	@Gender NVARCHAR(6), 
	@Nationality NVARCHAR(100),
	@Ethnicity NVARCHAR(50), 
	@PrimaryRecord NVARCHAR(6), 
	@EstCode NVARCHAR(3), 
	@OrgCode NVARCHAR(3)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @ClusterId INT
	DECLARE @RecordCount INT = 1
	DECLARE @OffenderId BIGINT = 0;
	DECLARE @OffenderToOffenderManagerMappingId BIGINT=150000;
	DECLARE @OmCode NVARCHAR(7);
	DECLARE @TeamCode NVARCHAR(7);
	DECLARE @Maxlength INT;
	DECLARE @PersonalDetailExist INT = 0;
	DECLARE @OmCodeExist INT = 0;


	IF EXISTS (SELECT TOP(1) UPCI2 FROM clusterdb.output.Clusters WHERE UPCI2 = @Identifier)
	BEGIN
		PRINT 'Record already exists';
		RETURN;
	END

	IF EXISTS(SELECT TOP(1) NOMSnumber FROM OfflocRunningPictureDb.OfflocRunningPicture.PersonalDetails where NOMSnumber = @NomisNumber)
	BEGIN
		PRINT 'Nomis already exists';
		RETURN;
	END

	IF EXISTS(SELECT TOP(1) CRN FROM DeliusRunningPictureDb.DeliusRunningPicture.Offenders where CRN = @Crn)
	BEGIN
		PRINT 'Delius already exists';
		RETURN;
	END

	SELECT TOP 1 @OffenderToOffenderManagerMappingId = Id FROM [DeliusRunningPictureDb].[DeliusRunningPicture].OffenderToOffenderManagerMappings ORDER BY Id DESC;
	SET @OffenderToOffenderManagerMappingId = @OffenderToOffenderManagerMappingId +1;
	
	SELECT @PersonalDetailExist = COUNT(1) FROM [OfflocRunningPictureDb].[OfflocRunningPicture].[PersonalDetails] WHERE [NOMSnumber] = @NomisNumber;

	SELECT TOP 1 @OffenderId = OffenderId FROM [DeliusRunningPictureDb].[DeliusRunningPicture].[Offenders] ORDER BY OffenderId DESC;
	SET @OffenderId = @OffenderId +1;
	--PRINT @OffenderId;
	
	--RETRIEVE OmCode if similar already exists
	--SELECT TOP 1 @OmCode = OmCode, @TeamCode = TeamCode from [DeliusRunningPictureDb].[DeliusRunningPicture].[OffenderManager] WHERE OmCode LIKE concat(@OrgCode, '%');
	--IF ISNULL(@OmCode, '') = ''
	--BEGIN
		--Add Randomly generated Characters, total length must not exceed 7, 
		--Ideally it should return same string based on the input string and some seed etc. 
	SET @Maxlength = (7-LEN(@OrgCode))+1;
	SET @OmCode = CONCAT(@OrgCode, SUBSTRING(CONVERT(varchar(40), NEWID()),0,@Maxlength));
	SET @TeamCode = SUBSTRING(@OmCode,0,LEN(@OmCode));
	--END
	--ELSE
	--BEGIN
	--	SET @OmCodeExist = 1;
	--END
	--To avoid duplicate OffenderManager
	SELECT @OmCodeExist = COUNT(1) FROM [DeliusRunningPictureDb].DeliusRunningPicture.OffenderManager WHERE OmCode=@OmCode and TeamCode=@TeamCode

	SELECT @ClusterId = ClusterId FROM [reference].[UPCI2] WHERE UPCI2 = @Identifier;

	IF @ClusterId IS NULL
	BEGIN
		INSERT INTO [reference].[UPCI2] (UPCI2) VALUES(@Identifier);
		SELECT @ClusterId = SCOPE_IDENTITY()
	END


	IF LEN(@NomisNumber)>0 and LEN(@Crn) > 0
	BEGIN
		SET @RecordCount=2;
	END

	--INSERT into Clusters table
	INSERT INTO [output].[Clusters]
           ([ClusterId]
           ,[UPCI2]
           ,[RecordCount]
           ,[ContainsInternalDupe]
           ,[ContainsLowProbabilityMembers]
           ,[PrimaryRecordName]
           ,[PrimaryRecordKey])
     VALUES
           (@ClusterId
		    ,@Identifier
			,@RecordCount
			,0
			,0
			,@PrimaryRecord
			,CASE WHEN @PrimaryRecord = 'NOMIS' THEN @NomisNumber ELSE @Crn END);

	IF(@RecordCount=2)
	BEGIN
		--Insert output.ClusterMembership table data

		--ADD NOMIS Record
		INSERT INTO [output].[ClusterMembership]
				   ([ClusterId]
				   ,[NodeName]
				   ,[NodeKey]
				   ,[ClusterMembershipProbability]
				   ,[HardLink])
			VALUES
				   (@ClusterId
				   ,'NOMIS'
				   ,@NomisNumber
				   ,CAST(RAND() AS DECIMAL(18,17))
				   ,0);

		--ADD DELIUS Record
		INSERT INTO [output].[ClusterMembership]
				   ([ClusterId]
				   ,[NodeName]
				   ,[NodeKey]
				   ,[ClusterMembershipProbability]
				   ,[HardLink])
			VALUES
				   (@ClusterId
				   ,'DELIUS'
				   ,@Crn
				   ,CAST(RAND() AS DECIMAL(18,17))
				   ,0);

		--Insert search.Attributes table data
		--ADD NOMIS Record
		INSERT INTO [search].[ClusterAttributes]
				   ([ClusterId]
				   ,[UPCI2]
				   ,[RecordSource]
				   ,[Identifier]
				   ,[PrimaryRecord]
				   ,[LastName]
				   ,[DOB])
				VALUES
				   (@ClusterId
				   ,@Identifier
				   ,'NOMIS'
				   ,@NomisNumber
				   ,Case when @PrimaryRecord = 'NOMIS' then 1 else 0 end
				   ,@LastName
				   ,CONVERT(DATE, @DateOfBirth, 103)
				   );

		--ADD DELIUS Record
		INSERT INTO [search].[ClusterAttributes]
				   ([ClusterId]
				   ,[UPCI2]
				   ,[RecordSource]
				   ,[Identifier]
				   ,[PrimaryRecord]
				   ,[LastName]
				   ,[DOB])
				VALUES
				   (@ClusterId
				   ,@Identifier
				   ,'DELIUS'
				   ,@Crn
				   ,Case when @PrimaryRecord = 'DELIUS' then 1 else 0 end
				   ,@LastName
				   ,CONVERT(DATE, @DateOfBirth, 103)
				   );

		INSERT INTO [output].[Clusters_EdgeProbabilities]
			([TempClusterId], [SourceKey], [TargetKey], [SourceName], [TargetName], [Probability])
		VALUES
			(-1, @NomisNumber, @Crn, 'NOMIS', 'DELIUS', CAST(RAND() AS DECIMAL(18,17)));

		IF @PersonalDetailExist = 0
		BEGIN
			--NOMIS 
			INSERT INTO [OfflocRunningPictureDb].[OfflocRunningPicture].[PersonalDetails]
						([NOMSnumber]
						,[Forename1]
						,[Surname]
						,[DOB]
						,[Gender]
						,[Nationality]
						,[IsActive])
					VALUES
						(@NomisNumber
						,@FirstName
						,@LastName
						,CONVERT(DATE, @DateOfBirth, 103)
						,@Gender
						,@Nationality
						,@IsActive);
		END
		--DELIUS 
		INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[Offenders]
					(
					[OffenderId]
					,[Id]
					,[FirstName]
					,[Surname]
					,[CRN]
					,[NOMISNumber]
					,[GenderCode]
					,[GenderDescription]
					,[DateOfBirth]
					,[NationalityDescription]
					,[Deleted])
				VALUES
					(
					@OffenderId
					,@OffenderId
					,@FirstName
					,@LastName
					,@Crn
					,@NomisNumber
					,CASE WHEN @Gender = 'Male' THEN 'M' ELSE 'F' END
					,@Gender
					,CONVERT(DATE, @DateOfBirth, 103)
					,@Nationality
					,'N');

		--NOMIS			
		INSERT INTO [OfflocRunningPictureDb].[OfflocRunningPicture].[OffenderAgencies]
					([NOMSnumber]
					,[EstablishmentCode]
					,[IsActive])
				VALUES
					(@NomisNumber
					,@EstCode
					,@IsActive);
		--DELIUS
		IF @OmCodeExist=0
		BEGIN
			INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[OffenderManager]
						([OmCode]
						,[OmForename]
						,[OmSurname]
						,[OrgCode]
						,TeamCode)
					VALUES
						(@OmCode
						,'Unallocated'
						,'Staff'
						,@OrgCode
						,@TeamCode)
		END
		INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[OffenderToOffenderManagerMappings]
					([OffenderId]
					,Id
					,[AllocatedDate]
					,[EndDate]
					,[OmCode]
					,[OrgCode]
					,[TeamCode]
					,[Deleted])
				VALUES
					(@OffenderId
					,@OffenderToOffenderManagerMappingId
					,DATEADD(MONTH, -2, getdate())
					,null
					,@OmCode
					,@OrgCode
					,@TeamCode
					,'N')

	END
	ELSE
	BEGIN
		IF @PrimaryRecord = 'NOMIS'
		BEGIN
			--Insert output.ClusterMembership table data
			INSERT INTO [output].[ClusterMembership]
					   ([ClusterId]
					   ,[NodeName]
					   ,[NodeKey]
					   ,[ClusterMembershipProbability]
					   ,[HardLink])
					VALUES
					   (@ClusterId
					   ,@PrimaryRecord
					   ,@NomisNumber
					   ,1
					   ,0);

			--Insert search.Attributes table data
			INSERT INTO [search].[ClusterAttributes]
					   ([ClusterId]
					   ,[UPCI2]
					   ,[RecordSource]
					   ,[Identifier]
					   ,[PrimaryRecord]
					   ,[LastName]
					   ,[DOB])
					VALUES
					   (@ClusterId
					   ,@Identifier
					   ,@PrimaryRecord
					   ,@NomisNumber
					   ,1
					   ,@LastName
					   ,CONVERT(DATE, @DateOfBirth, 103)
					   );
			IF @PersonalDetailExist = 0
			BEGIN
				--NOMIS 
				INSERT INTO [OfflocRunningPictureDb].[OfflocRunningPicture].[PersonalDetails]
						([NOMSnumber]
						,[Forename1]
						,[Surname]
						,[DOB]
						,[Gender]
						,[Nationality]
						,[IsActive])
					VALUES
						(@NomisNumber
						,@FirstName
						,@LastName
						,CONVERT(DATE, @DateOfBirth, 103)
						,@Gender
						,@Nationality
						,@IsActive);
			END			
			--NOMIS			
			INSERT INTO [OfflocRunningPictureDb].[OfflocRunningPicture].[OffenderAgencies]
					([NOMSnumber]
					,[EstablishmentCode]
					,[IsActive])
				VALUES
					(@NomisNumber
					,@EstCode
					,@IsActive);
					
		END

		IF @PrimaryRecord = 'DELIUS'
		BEGIN
			--Insert output.ClusterMembership table data
			INSERT INTO [output].[ClusterMembership]
					   ([ClusterId]
					   ,[NodeName]
					   ,[NodeKey]
					   ,[ClusterMembershipProbability]
					   ,[HardLink])
					VALUES
					   (@ClusterId
					   ,@PrimaryRecord
					   ,@Crn
					   ,1
					   ,0);

			--Insert search.Attributes table data
			INSERT INTO [search].[ClusterAttributes]
					   ([ClusterId]
					   ,[UPCI2]
					   ,[RecordSource]
					   ,[Identifier]
					   ,[PrimaryRecord]
					   ,[LastName]
					   ,[DOB])
					VALUES
					   (@ClusterId
					   ,@Identifier
					   ,@PrimaryRecord
					   ,@Crn
					   ,1
					   ,@LastName
					   ,CONVERT(DATE, @DateOfBirth, 103)
					   );

			INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[Offenders]
					(
					[OffenderId]
					,[Id]
					,[FirstName]
					,[Surname]
					,[CRN]
					,[GenderCode]
					,[GenderDescription]
					,[DateOfBirth]
					,[NationalityDescription]
					,[Deleted])
				VALUES
					(
					@OffenderId
					,@OffenderId
					,@FirstName
					,@LastName
					,@Crn
					,CASE WHEN @Gender = 'Male' THEN 'M' ELSE 'F' END
					,@Gender
					,CONVERT(DATE, @DateOfBirth, 103)
					,@Nationality
					,'N');

			IF @OmCodeExist=0
			BEGIN
				INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[OffenderManager]
						([OmCode]
						,[OmForename]
						,[OmSurname]
						,[OrgCode]
						,TeamCode)
					VALUES
						(@OmCode
						,'Unallocated'
						,'Staff'
						,@OrgCode
						,@TeamCode)
			END

			INSERT INTO [DeliusRunningPictureDb].[DeliusRunningPicture].[OffenderToOffenderManagerMappings]
					([OffenderId]
					,Id
					,[AllocatedDate]
					,[EndDate]
					,[OmCode]
					,[OrgCode]
					,[TeamCode]
					,[Deleted])
				VALUES
					(@OffenderId
					,@OffenderToOffenderManagerMappingId
					,DATEADD(MONTH, -2, getdate())
					,null
					,@OmCode
					,@OrgCode
					,@TeamCode
					,'N')
		END
	END

END

