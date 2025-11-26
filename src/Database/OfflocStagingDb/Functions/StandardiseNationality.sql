CREATE FUNCTION [OfflocStaging].[Fn.StandardiseNationality] (@nationality nvarchar(100))
returns nvarchar(100) with schemabinding
as
begin

	--map_nationality contains a list of all observed nationalities along with eea eligibility
	--if the nationality is not in this list, then null

	declare @return nvarchar(100)

	select @return = [Nationality] from [OfflocStaging].[Lookups.Nationality] where [Nationality] = @nationality
	

	return @return

end