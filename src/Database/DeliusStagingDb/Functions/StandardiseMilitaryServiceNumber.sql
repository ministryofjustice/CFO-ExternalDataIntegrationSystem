
CREATE FUNCTION [DeliusStaging].[Fn.StandardiseMilitaryServiceNumber] (@militaryservicenumber nvarchar(100))
returns nvarchar(10) with schemabinding
as
begin

	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	--military service number format:
	--length is variable
	--(any number up to a length of 8)
	--navy numbers can also start and end with a letter

	--remove common text
	set @militaryservicenumber = replace(@militaryservicenumber,'army','')
	set @militaryservicenumber = replace(@militaryservicenumber,'navy','')
	set @militaryservicenumber = replace(@militaryservicenumber,'raf','')

	--format to uppercase and remove all non-alphanumeric chars
	set @militaryservicenumber = upper(@militaryservicenumber)
	
	while patindex('%[^a-z0-9]%', @militaryservicenumber) > 0 
		set @militaryservicenumber = stuff(@militaryservicenumber, patindex('%[^a-z0-9]%', @militaryservicenumber), 1, '')

	--possible navy number
	if isnumeric(@militaryservicenumber) = 0
	begin
		--expect format [a][5 to 8 digit num][a]
		if not(@militaryservicenumber like '[a-z][0-9][0-9][0-9][0-9][0-9][a-z]'
			or @militaryservicenumber like '[a-z][0-9][0-9][0-9][0-9][0-9][0-9][a-z]'
			or @militaryservicenumber like '[a-z][0-9][0-9][0-9][0-9][0-9][0-9][0-9][a-z]'
			or @militaryservicenumber like '[a-z][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][a-z]')
			begin
				set @militaryservicenumber = null
			end
	end
	else
	begin
		--sense check on number's length
		if not(len(@militaryservicenumber) between 5 and 8)
		begin
			set @militaryservicenumber = null
		end
	end

	return @militaryservicenumber

end
GO
