IF '$(PopulateReferenceTables)' = 'True'
BEGIN
    EXEC processing.PopulateReferenceTables;
END