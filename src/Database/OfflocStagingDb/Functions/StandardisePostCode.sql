CREATE FUNCTION [OfflocStaging].[Fn.StandardisePostCode] (@postcode nvarchar(100))
returns nvarchar(8) with schemabinding
as
begin

--for matching purposes, here we validate the postcode's format and remove any communal or dummy postcodes.
--we do not validate that the postcode is in use by royal mail.

--format
--a# #aa
--a## #aa
--aa# #aa
--aa## #aa
--a#a #aa
--aa#a #aa
--the second part of the postcode (#aa) never contains {c i k m o v}

--exemptions - a list of postcodes is held in map_exemptpostcodes that are deemed as not unique enough for matching purposes.
--these postcodes are generally prisons, probation offices, courts and hostels.
--the procedure sprefreshexemptpostcodes appends postcodes to this list based on the current daily feed. see this for more info on how exempt postcodes are determined.


	if (
			patindex('[a-z][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
			or patindex('[a-z][0-9][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz', @postcode) = 1
			or patindex('[a-z][a-z][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
			or patindex('[a-z][a-z][0-9][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
			or patindex('[a-z][0-9][a-z] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
			or patindex('[a-z][a-z][0-9][a-z] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
		)
	begin
		return @postcode
	end


	--format to uppercase and remove all non-alphanumeric chars
	set @postcode = upper(@postcode)

	--get rid of everything that isn't a letter or a number
	while patindex('%[^a-z0-9]%', @postcode) > 0 
		set @postcode = stuff(@postcode, patindex('%[^a-z0-9]%', @postcode), 1, '')

	
	if len(@postcode) < 4 return null


	-- re-add a single space in the old middle
	set @postcode = left(@postcode, len(@postcode) - 3) + ' ' + right(@postcode, 3)
	
	--length must be between 5 and 7
	if not len(@postcode) between 5 and 7
	begin
		return null;
	end
	else
	begin 
		if (
				patindex('[a-z][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
				or patindex('[a-z][0-9][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz', @postcode) = 1
				or patindex('[a-z][a-z][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
				or patindex('[a-z][a-z][0-9][0-9] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
				or patindex('[a-z][0-9][a-z] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
				or patindex('[a-z][a-z][0-9][a-z] [0-9][abdefghjlnpqrstuwxyz][abdefghjlnpqrstuwxyz]', @postcode) = 1
			)
		begin
			return @postcode
		end
		begin
			return null
		end
	end
	return null
end

