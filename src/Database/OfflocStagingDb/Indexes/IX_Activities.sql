CREATE CLUSTERED INDEX [IX_Activities] ON [OfflocStaging].[Activities] 
	(NOMSnumber, Activity, Location, StartHour, StartMin, EndHour, EndMin);
