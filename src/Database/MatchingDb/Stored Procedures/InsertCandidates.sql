CREATE PROCEDURE [Matching].[InsertCandidates] (@jsonQueries NVARCHAR(MAX))
AS
BEGIN

	SET NOCOUNT ON;

	DECLARE 
		@ID INT,
		@MaxIterator INT,
		@Iterator INT,
		@Query NVARCHAR(MAX),
		@FinalQuery NVARCHAR(MAX)

	CREATE TABLE #BlockingQueryTable ( Iterator INT IDENTITY(1, 1), [Order] INT, BlockingQuery NVARCHAR(MAX));
   
	BEGIN TRY
	
		--CLEAR CURRENT Blocks, Candidates and Matches
		TRUNCATE TABLE Matching.Matches;

		ALTER TABLE [Matching].[Matches] DROP CONSTRAINT [FK_Matches_Candidates]
		ALTER TABLE [Matching].[Candidates] DROP CONSTRAINT [FK_Candidates_Blocks]

		TRUNCATE TABLE Matching.Candidates;
		TRUNCATE TABLE Matching.Blocks;

		ALTER TABLE [Matching].[Matches]  WITH CHECK ADD  CONSTRAINT [FK_Matches_Candidates] FOREIGN KEY([CandidateId])
		REFERENCES [Matching].[Candidates] ([CandidateId])
		ALTER TABLE [Matching].[Matches] CHECK CONSTRAINT [FK_Matches_Candidates]

		ALTER TABLE [Matching].[Candidates]  WITH CHECK ADD  CONSTRAINT [FK_Candidates_Blocks] FOREIGN KEY([BlockId])
		REFERENCES [Matching].[Blocks] ([BlockId])
		ALTER TABLE [Matching].[Candidates] CHECK CONSTRAINT [FK_Candidates_Blocks];

		--Populate temporary tables with queries from configuration received as json variable
		INSERT INTO #BlockingQueryTable ( [Order], BlockingQuery )
		SELECT [Order], BlockingQuery
		FROM OPENJSON(@jsonQueries) WITH (
			[Order] NVARCHAR(50) '$.Order',
			BlockingQuery NVARCHAR(MAX) '$.BlockingQuery'
		);

		--Set Iterators
		SELECT 
			@MaxIterator = MAX(Iterator), 
			@Iterator = 1
		FROM 
			#BlockingQueryTable;

		--Iterate through the queries and populate the final query to run to poulate the blocks
		WHILE @Iterator <= @MaxIterator 
			BEGIN 
				SET @Query ='';

				SELECT 
					@Query = BlockingQuery 
				FROM 
					#BlockingQueryTable 
				WHERE
					Iterator = @Iterator;
		
				--PRINT @Query;

				IF @Iterator = @MaxIterator
					SET @FinalQuery =  CONCAT(@FinalQuery, @Query);
				ELSE
					SET @FinalQuery =  CONCAT(@FinalQuery, @Query, ' UNION ');

				SET @Iterator = @Iterator +1;
			END;
	
			SET @FinalQuery = CONCAT('INSERT INTO MatchingDb.Matching.Blocks SELECT PAIRS.SourceKey, PAIRS.SourceName, PAIRS.TargetKey, PAIRS.TargetName FROM (', @FinalQuery, ') AS [PAIRS];');
			--print @FinalQuery;
			EXEC (@FinalQuery);

		--Populate Candidates based on the Blocks
		INSERT INTO [Matching].[Candidates]
		(
			BlockId,
			[l_SourceName],
			[l_SourceKey],
			[l_NOMISNumber],
			[l_PNCNumber],
			[l_FirstName],
			[l_SecondName],
			[l_LastName],
			[l_DateOfBirth],
			[l_CRONumber],
			[l_Gender],
			[r_SourceName],
			[r_SourceKey],
			[r_PNCNumber],
			[r_FirstName],
			[r_SecondName],
			[r_LastName],
			[r_DateOfBirth],
			[r_NOMISNumber],
			[r_CRONumber],
			[r_Gender]
		)
		SELECT [BlockId],
			   [l_SourceName],
			   [l_SourceKey],
			   [l_NOMISNumber],
			   [l_PNCNumber],
			   [l_FirstName],
			   [l_SecondName],
			   [l_LastName],
			   [l_DateOfBirth],
			   [l_CRONumber],
			   [l_Gender],
			   [r_SourceName],
			   [r_SourceKey],
			   [r_PNCNumber],
			   [r_FirstName],
			   [r_SecondName],
			   [r_LastName],
			   [r_DateOfBirth],
			   [r_NOMISNumber],
			   [r_CRONumber],
			   [r_Gender]
		FROM Matching.ListBlockedCandidates;

	END TRY

	BEGIN CATCH
	SELECT ERROR_NUMBER() AS ErrorNumber ,
		   ERROR_MESSAGE() AS ErrorMessage;
	END CATCH;

	DROP TABLE #BlockingQueryTable


END