IF '$(PopulateReferenceTables)' = 'True'
BEGIN
    RAISERROR (N'Executing post deployment script. This may take a while...', 0, 0) WITH NOWAIT;
    EXEC processing.PopulateReferenceTables;
END