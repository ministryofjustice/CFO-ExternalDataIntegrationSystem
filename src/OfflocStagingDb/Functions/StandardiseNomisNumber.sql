CREATE FUNCTION [OfflocStaging].[Fn.StandardiseNomisNumber] (@nomisnumber nvarchar(100))
returns nvarchar(7) with schemabinding
as
begin

--nomis number format:
--length = 7
--format ['a'####aa] (always starts with the letter a)

--format to uppercase and remove all non-alphanumeric chars
set @nomisnumber = upper(@nomisnumber)
while patindex('%[^a-z0-9]%', @nomisnumber) > 0 set @nomisnumber = stuff(@nomisnumber, patindex('%[^a-z0-9]%', @nomisnumber), 1, '')

--if length is not 7 then null
if len(@nomisnumber) <> 7
	begin
		set @nomisnumber = null
	end
else
	begin
		--if incorrect format then null
		if @nomisnumber not like '[a][0-9][0-9][0-9][0-9][a-z][a-z]'
			begin
				set @nomisnumber = null
			end
		else
			begin
				--ignore dummy numbers
				if @nomisnumber = 'a9999aa'
					begin
						set @nomisnumber = null
					end
				if @nomisnumber = 'A0000AA'
					begin
						set @nomisnumber = null
					end
			end
	end

return @nomisnumber

end
