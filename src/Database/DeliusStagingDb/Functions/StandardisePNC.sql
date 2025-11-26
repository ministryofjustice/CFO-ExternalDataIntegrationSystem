CREATE FUNCTION [DeliusStaging].[Fn.StandardisePNC] (@pnc nvarchar(100))
returns nvarchar(13) with schemabinding
as
begin
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	declare @prefix nvarchar(100), @number nvarchar(100), @suffix nvarchar(100), @return nvarchar(13)

	--format to uppercase and remove all non-alphanumeric chars except '/'
	set @pnc = upper(@pnc)
	while patindex('%[^a-z0-9/]%', @pnc) > 0 set @pnc = stuff(@pnc, patindex('%[^a-z0-9/]%', @pnc), 1, '')

	--split pnc into composite parts

	--if no slash then set the return to null
	if charindex('/',@pnc,1) = 0
		begin
			set @return = null
		end
	else
		begin
			--prefix is everything in front of the slash and should be a number of length four (the year)
			set @prefix = left(@pnc,charindex('/',@pnc,1)-1)
			set @pnc = right(@pnc,len(@pnc)-charindex('/',@pnc,1)) --trim prefix off the given pnc for later use
			if case when not(patindex('%[^0-9]%',@prefix) between 1 and len(@prefix)) then 1 else 0 end = 1 --isnumeric(@prefix) = 1 - Ammended by AB 10/09/2018
				begin
					if len(@prefix) = 4 or len(@prefix) = 2 --allowable year formats
						begin
							set @prefix = right(@prefix,2) --take last two digits
							--populate the start of year based on current year
							if cast(@prefix as int) <= cast(right(year(getdate()),2) as int)
								begin
									set @return = '20' + @prefix + '/'
								end
							else
								begin
									set @return = '19' + @prefix + '/'
								end
						end
					else
						begin
							set @return = null --wrong number of digits, cannot derive year from this
						end
				end
			else
				begin
					set @return = null --prefix is not numeric and thus not a year
				end

			--number is of length seven but can be less (due to leading zeros being missed) but must be at least length of one
			if patindex('%[^0-9]%',@pnc) between 2 and 8
				begin
					set @number = left(@pnc,patindex('%[^0-9]%',@pnc)-1)
					set @suffix = substring(@pnc,patindex('%[^0-9]%',@pnc),len(@pnc)-patindex('%[^0-9]%',@pnc)+1) --for use later
					--pad number with leading zeros so of length 7
					set @number = right('000000'+@number,7)
					set @return = @return + @number
				end
			else
				begin
					set @return = null
				end

			--suffix is a single letter
			if @suffix is null or len(@suffix) <> 1
				begin
					set @return = null
				end
			else
				begin
					set @return = @return + @suffix
				end
		end

		--finally, if pnc = 2033/0993399h or 1933/0993399h, then null (these appear to be dummy/shared/temporary pncs)
		if @return in ('2033/0993399h','1933/0993399h')
			begin
				set @return = null
			end

	return @return

end
GO


