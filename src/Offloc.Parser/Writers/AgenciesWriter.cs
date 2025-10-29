namespace Offloc.Parser.Writers;

//Custom writer that ensures that only a single table of agencies is maintained.
//If this form of custom discretionary writer has to be created consider creating 
//a special class and configuration for writers that should ensure uniqueness.
public class AgenciesWriter : WriterBase, IWriter
{
    private Dictionary<string, string> agencyRecords = new Dictionary<string, string>();
    private int[] relevantFields = new int[] { 2, 1 };
    
    public AgenciesWriter(string path, int[] redundantFields) 
        : base($"{path}/Agencies.txt")
    {
        for (int i = 0; i < relevantFields.Length; i++)
        {
            relevantFields[i] -= redundantFields.Where(f => f < relevantFields[i]).Count();
        }
    }

    public async Task WriteAsync(string NOMSNumber, string[] contents)
    {
        //NOMSNumber is not needed for Agencies.
        string? agencyCode;
        bool agencyFound = agencyRecords.TryGetValue(contents[relevantFields[0]], out agencyCode);

        if (!agencyFound)
        {
            string[] fields = [contents[relevantFields[0]], contents[relevantFields[1]]];
            agencyRecords.Add(fields[0], fields[1]);

            await StreamWriter!.WriteLineAsync(string.Join('|', fields));
        }
    }
}
