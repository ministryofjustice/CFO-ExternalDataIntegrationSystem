CREATE FUNCTION [DeliusStaging].[Fn.StandardisePhoneNumber] (
	@phonenumber nvarchar(100),
	@removenongeographic bit = 1)
returns nvarchar(11) with schemabinding
as
begin
	SET ANSI_NULLS ON
	SET QUOTED_IDENTIFIER ON

	--remove all non-numeric chars
	while patindex('%[^0-9]%', @phonenumber) > 0 
		set @phonenumber = stuff(@phonenumber, patindex('%[^0-9]%', @phonenumber), 1, '')

	-- remove anything that doesn't start 01-9
	if patindex('0[1-9]%', @phonenumber) = 0 or len(@phonenumber) > 11
		set @phonenumber = null

	if @removenongeographic = 1
	begin
	--check format
	if not((@phonenumber like '01%' and len(@phonenumber) in (9,10,11))
		or (@phonenumber like '02%' and len(@phonenumber) in (9,10,11))
		or (@phonenumber like '07%' and len(@phonenumber) = 11))
		begin
			set @phonenumber = null
		end
	end 

	return @phonenumber

end

GO