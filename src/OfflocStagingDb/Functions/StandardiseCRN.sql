CREATE FUNCTION [OfflocStaging].[Fn.StandardiseCRN] (@crn nvarchar(100))
returns nvarchar(7)
with schemabinding
as
begin

--crn number format:
--length = 7
--format [a######]

--format to uppercase and remove all non-alphanumeric chars
set @crn = upper(@crn)
while patindex('%[^a-z0-9]%', @crn) > 0 set @crn = stuff(@crn, patindex('%[^a-z0-9]%', @crn), 1, '')

--if length is not 7 then null
if len(@crn) <> 7
	begin
		set @crn = null
	end
else
	begin
		--if incorrect format then null
		if @crn not like '[a-z][0-9][0-9][0-9][0-9][0-9][0-9]'
			begin
				set @crn = null
			end
	end

return @crn

end