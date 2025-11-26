CREATE FUNCTION [OfflocStaging].[Fn.StandardiseEthnicity] (@ethnicity nvarchar(100))
returns nvarchar(100)
with schemabinding
as
begin

	--map_ethnicity contains a list of all observed ethnicities along with a consistant terminology
	--if the ethnicity is not in this list, then recode to 'refused / not stated'

	declare @return nvarchar(100)

	select 
		@return = [StandardEthnicity]
	from 
		--dms.dbo.lu_ethnicitystandard 
		[OfflocStaging].[Lookups.EthnicityStandard]
	where 
		[EthnicityID] = (select [EthnicityID] from [OfflocStaging].[Lookups.Ethnicity] where [SourceEthnicity] = @ethnicity)

	return isnull(@return,'refused / not stated / new ethnicity')

end