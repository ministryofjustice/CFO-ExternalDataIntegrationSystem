CREATE FUNCTION [DeliusRunningPicture].[Fn.StandardisePrisonNumber] (@prisonnumber nvarchar(100))
returns nvarchar(6) with schemabinding
as
begin
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	--prison number format:
	--length = 6
	--either [######], [a#####] or [aa####]
	--update 11/11/2016 - or [#####a]

	declare @return nvarchar(6)

	--format to uppercase and remove all non-alphanumeric chars
	set @prisonnumber = upper(@prisonnumber)
	while patindex('%[^a-z0-9]%', @prisonnumber) > 0 set @prisonnumber = stuff(@prisonnumber, patindex('%[^a-z0-9]%', @prisonnumber), 1, '')

	--if prison number is not of length 6 then invalid so null
	if len(@prisonnumber) = 6
		begin
			--check if format is correct
			if @prisonnumber like '[a-z][a-z][0-9][0-9][0-9][0-9]' or
			   @prisonnumber like '[a-z][0-9][0-9][0-9][0-9][0-9]' or
			   @prisonnumber like '[0-9][0-9][0-9][0-9][0-9][0-9]' or
			   @prisonnumber like '[0-9][0-9][0-9][0-9][0-9][a-z]' 
				begin
					set @return = @prisonnumber
				end
			else
				begin
					set @return = null
				end

		end
	else
		begin
			set @return = null
		end

	return @return

end
GO