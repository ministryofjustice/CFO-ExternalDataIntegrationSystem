CREATE FUNCTION [OfflocStaging].[Fn.StandardiseCRO] (@cro nvarchar(100))
returns nvarchar(10)
with schemabinding
as
begin

--cro number format:
--length = 10
--char 1 to 6 are numeric (leading zeros often omitted)
--char 7 is a slash (/)
--char 8 & 9 are numeric
--char 10 is a-z

declare @prefix nvarchar(100), @suffix nvarchar(100), @return nvarchar(10)

--format to uppercase and remove all non-alphanumeric chars except '/'
set @cro = upper(@cro)
while patindex('%[^a-z0-9/]%', @cro) > 0 set @cro = stuff(@cro, patindex('%[^a-z0-9/]%', @cro), 1, '')

--split cro into composite parts

--if no slash then set the return to null
if charindex('/',@cro,1) = 0
	begin
		set @return = null
	end
else
	begin
		--prefix is everything in front of the slash and should be a number of length six
		set @prefix = left(@cro,charindex('/',@cro,1)-1)
		set @suffix = right(@cro,len(@cro)-charindex('/',@cro,1)) --trim prefix off the given cro for later use
		if isnumeric(@prefix) = 1
			begin
				--pad suffix with leading zeros if required
				set @prefix = right('000000'+@prefix,6)

				--if prefix is just zeros then null
				if @prefix = '000000'
					begin
						set @return = null 
					end
				else
					begin
						set @return = @prefix
					end
			end
		else
			begin
				set @return = null --prefix is not numeric
			end

		--suffix is of the format [##a] (trailing zeros are not expected)
		if @suffix is null or len(@suffix) <> 3
			begin
				set @return = null
			end
		else
			begin
				if @suffix like '%[0-9][0-9][a-z]%'
					begin
						set @return = @return + '/' + @suffix
					end
			end
	end

return @return

end
