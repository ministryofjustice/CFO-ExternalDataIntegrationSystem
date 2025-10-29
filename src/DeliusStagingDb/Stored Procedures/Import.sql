-- =============================================
-- Author:		Carl Sixsmith
-- Create date: 13/04/2015
-- Description:	Runs the import of tables for a given path
-- =============================================

CREATE PROCEDURE [dbo].[Import] 
	-- Add the parameters for the stored procedure here
	@Path varchar(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	EXECUTE sp_configure 'show advanced options', 1;
	RECONFIGURE

	EXECUTE sp_configure 'xp_cmdshell', 1;
	RECONFIGURE

	--declare @path varchar(1000)

	--select @path = FileLocation from StateMachineContext.dbo.ImportFile where id = @FileId

	BEGIN TRANSACTION 
	BEGIN TRY
		DECLARE @result INT, 
				@cmd VARCHAR(8000)

		if right(@Path,1) <> '\'
		begin
			set @Path = @Path + '\'
		end

		SET @cmd = 'DIR ' + @Path + ' /a-d /b'

		DECLARE @files TABLE (
			[filename] VARCHAR(255) null
		)

		INSERT INTO @files ([filename])
		EXECUTE @result = master.dbo.xp_cmdshell @cmd /*Lists all files in the directory. */
	
		IF (select count(*) FROM @Files where filename is not null) = 0
		begin
			declare @err nvarchar(400)
			set @err = N'No files found in' + @Path
			;throw 51000, @err, 1;
		end	


		DECLARE @tablename SYSNAME, @sql NVARCHAR(4000)

		SET DATEFORMAT DMY

		DECLARE c CURSOR FAST_FORWARD READ_ONLY FORWARD_ONLY
		FOR SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'

		OPEN c

		FETCH NEXT FROM c INTO @tablename

		WHILE @@FETCH_STATUS = 0
		BEGIN
	
			IF EXISTS ( SELECT * FROM @files WHERE [filename] = @tablename + '.txt' )
			BEGIN
				SET @sql = 'bulk insert ' + @tablename + ' FROM  ''' + @Path + @tablename + '.txt'' WITH (FieldTerminator = ''|'', datafiletype=''widechar'')'
				EXEC sp_executesql @sql
				
				if exists (select * from INFORMATION_SCHEMA.columns where column_name = 'id' and table_name = @tablename)
					BEGIN
						set @sql = 'CREATE INDEX [NonClustered_' + @tablename + '_id] on dbo.[' + @tablename + '] (OffenderId asc) '
						exec sp_executesql @sql
					END
				END
			--ELSE
			--BEGIN
				--select @msg = 'no file found for ' + @tablename 
				--exec EventLogger.dbo.Info @Message = @msg
			--END
	
	
			FETCH NEXT FROM c INTO @tablename
		END
		CLOSE c
		DEALLOCATE c
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;

		--select @msg = ERROR_MESSAGE()
		--exec EventLogger.dbo.Error @Message = @msg;

		THROW
	END CATCH

	EXECUTE sp_configure 'show advanced options', 0
	RECONFIGURE

	EXECUTE sp_configure 'xp_cmdshell', 0
	RECONFIGURE
END