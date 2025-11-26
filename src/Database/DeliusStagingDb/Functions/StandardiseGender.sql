CREATE FUNCTION [DeliusStaging].[Fn.StandardiseGender] (@gender nvarchar(100))
returns nvarchar(6) with schemabinding
as
begin

	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON
	--gender can be male, female or not specified/indeterminate
	--gender can also be coded to m, f etc

	--format to uppercase and remove all non-alpha chars
	set @gender = upper(@gender)
	while patindex('%[^a-z]%', @gender) > 0 set @gender = stuff(@gender, patindex('%[^a-z]%', @gender), 1, '')


	--if starts with 'm' then 'male'
	if left(@gender,1) = 'm'
		begin
			set @gender = 'male'
		end
	else
		begin
			--if starts with 'f' then 'female'
			if left(@gender,1) = 'f'
				begin
					set @gender = 'female'
				end
			else
				begin
					set @gender = 'other' --ammended by AB 14/03/2018 (was null)
				end
		end

	return @gender

end
GO