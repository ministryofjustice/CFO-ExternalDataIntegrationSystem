CREATE FUNCTION [OfflocStaging].[Fn.StandardiseName](@name nvarchar(140))
returns nvarchar(140) with schemabinding
as
begin
	declare @return nvarchar(140)

	set @return = replace(@name, '-', ' ')

	while patindex('%[^a-z ]%',@return) > 0 
		set @return = stuff(@return, patindex('%[^a-z ]%', @return), 1, '')

	while patindex('%  %',@return) > 0
		set @return = replace(@return,'  ',' ')
	
	return rtrim(ltrim(@return))
end