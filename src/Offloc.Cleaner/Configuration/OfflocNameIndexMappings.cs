
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Offloc.Cleaner.Configuration;

public static class OfflocNameIndexMappings
{
    public static Dictionary<string, int> NameIndexDictionary = new Dictionary<string, int>();

    private const string fileName = "IndexFieldMappings.xlsx";

    //This will need to be tweaked when 257 is merged.
    //private string filePath = "";

    static OfflocNameIndexMappings()
    {
        string calculatedPath = DeterminePath();

        NameIndexDictionary.EnsureCapacity(100);
        PopulateDictionary(calculatedPath);
    }

    private static string DeterminePath()
    {
        //Hacky conditional to determine path based on whether app is running 
        //locally or in container.
        string targetPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        
        string calculatedPath = $"{targetPath}/Configuration/{fileName}";

        return calculatedPath;
    }

    private static void PopulateDictionary(string filePath)
    {
        SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(filePath, false);

        using (spreadsheetDocument)
        {
            WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart!;
            WorksheetPart worksheetPart = workbookPart!.WorksheetParts.First();
            SheetData sheetData = worksheetPart.Worksheet.Elements<SheetData>().First();

            var sharedStrings = workbookPart.GetPartsOfType<SharedStringTablePart>()
                .FirstOrDefault()!.SharedStringTable ?? throw new ApplicationException($"Shared string table not found for file: {filePath}");
            var rows = sheetData.Elements<Row>().ToArray();

            for (short i = 0; i < rows.Count(); i++)
            {
                if (!rows[i].HasChildren)
                {
                    break;
                }
                var cells = rows[i].Elements<Cell>().ToArray();

                if (cells.Count() != 2)
                {
                    throw new ApplicationException("Row didn't contain the expected number of columns (2).");
                }
                else
                {
                    //Normalization to prevent case-sensitivity errors from config.
                    var key = sharedStrings.ElementAt(short.Parse(cells[1]!.InnerText)).InnerText.Trim().ToLower();
                    short value = (short)short.Parse(cells[0].CellValue!.Text);

                    //Check if key is already added.
                    if (!NameIndexDictionary.ContainsKey(key))
                    {
                        NameIndexDictionary.Add(key, value);
                    }
                }
            }
            var temp = NameIndexDictionary.Values.Order();
        }
    }
}
