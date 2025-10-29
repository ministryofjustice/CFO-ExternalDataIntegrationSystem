namespace Offloc.Parser.Services.TrimmerContext.SecondaryContexts;

public class AddressFieldsContext
{
    public int startingIndex = 77;
    public int endingIndex = 116;

    public AddressFieldsContext(int[] redundantFields)
    {
        startingIndex -= redundantFields.Where(f => f < startingIndex).Count();
        endingIndex -= redundantFields.Where(f => f < endingIndex).Count();
    }
}
