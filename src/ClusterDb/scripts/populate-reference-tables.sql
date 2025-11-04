IF '$(PopulateReferenceTables)' = 'True'
BEGIN
    PRINT('Executing post deployment script. This may take a while...')
    EXEC processing.PopulateReferenceTables;
END