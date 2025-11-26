CREATE FUNCTION [DeliusStaging].[Fn.StandardiseNINO] (@nino nvarchar(100))
returns nvarchar(9) with schemabinding
as
begin

	--national insurance number format:
	--length = 9
	--char 1 & 2 are alpha and valid such as in map_ninovalidprefixes
	--char 3 - 8 are numeric
	--char 9 is {a,b,c,d or a space} and is not required for matching (unique without suffix)

	declare @prefix nvarchar(2), 
			@number nvarchar(6), 
			@suffix nvarchar(1), 
			@return nvarchar(9)

	--format to uppercase and remove all non-alphanumeric chars
	set @nino = upper(@nino)

	while patindex('%[^a-z0-9]%', @nino) > 0 
		set @nino = stuff(@nino, patindex('%[^a-z0-9]%', @nino), 1, '')

	--if nino is not of length 8 or 9 then invalid so null
	if len(@nino) in (8,9)
	begin
		--split nino up into composite parts for validation
		set @prefix = left(@nino,2)
		set @number = substring(@nino,3,6)
		set @suffix = substring(@nino,9,1)

		if isnumeric(@number) = 0 
		--or @suffix not in ('a','b','c','d','')
			or @prefix not in (select [Prefix] from [DeliusStaging].[Merging.NINOValidPrefixes])
		begin
			set @return = null
		end
		else
		begin
			--nino is correctly formatted. ommit the suffix as not needed for matching.
			set @return = @prefix+@number
		end
	end
	else
	begin
		set @return = null
	end

	return @return
end
