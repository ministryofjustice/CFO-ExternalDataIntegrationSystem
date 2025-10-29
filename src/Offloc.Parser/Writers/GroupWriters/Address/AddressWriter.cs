using Offloc.Parser.Services.TrimmerContext.SecondaryContexts;
using System.Text;

namespace Offloc.Parser.Writers.GroupWriters.Address;

//Could be classed as a group writer or discretionary writer.
public class AddressWriter : WriterBase, IWriter
{
    private const string tableName = "Addresses";
    private readonly AddressFieldsContext addressFieldsContext;

    //Instance variables so it's easier to split up writing functionility into different methods.
    private string[] dischargeContent = { };
    private string[] receptionContent = { };
    private string[] homeContent = { };
    private string[] NOKContent = { };
    private string[] probationContent = { };

    public AddressWriter(string path, AddressFieldsContext addressFieldsContext)
        : base($"{path}/{tableName}.txt")
    {
        this.addressFieldsContext = addressFieldsContext;
    }

    public async Task WriteAsync(string NOMSNumber, string[] contents)
    {
        string[] subContents = contents[addressFieldsContext.startingIndex..(addressFieldsContext.endingIndex + 1)];

        dischargeContent = subContents[0..8].Prepend("").ToArray();
        receptionContent = subContents[8..16].Prepend("").ToArray();
        homeContent = subContents[16..23].Prepend("").Prepend("").ToArray();
        NOKContent = subContents[23..32];

        //For parsing alignment testing.
        string offenderManager = subContents[32];

        probationContent = subContents[33..].Prepend("").Prepend("").ToArray();

        await ConditionalWrite(dischargeContent, NOMSNumber, TAddress.Discharge);
        await ConditionalWrite(receptionContent, NOMSNumber, TAddress.Reception);
        await ConditionalWrite(homeContent, NOMSNumber, TAddress.Home);
        await ConditionalWrite(NOKContent, NOMSNumber, TAddress.NOK);
        await ConditionalWrite(probationContent, NOMSNumber, TAddress.Probation);
    }

    private async Task ConditionalWrite(IEnumerable<string> subContent, string NOMSNumber, TAddress addressType)
    {
        //Only writes to text file if one of the columns isn't empty.
        if (subContent.Any(s => !string.IsNullOrEmpty(s)))
        {
            string s = string.Join('|', subContent.Prepend(addressType.ToString()).Prepend($"{NOMSNumber}"));
            await StreamWriter!.WriteLineAsync(s);
        }
    }
}
