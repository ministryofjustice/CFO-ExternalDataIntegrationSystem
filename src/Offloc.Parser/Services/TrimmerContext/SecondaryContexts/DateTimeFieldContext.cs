namespace Offloc.Parser.Services.TrimmerContext.SecondaryContexts;

public class DateTimeFieldContext
{
    public int[] datetimes = { 64, 142, 31 };

    public DateTimeFieldContext(int[] redundantFields)
    {
        datetimes = RecalculateIndexes(redundantFields);
    }

    private int[] RecalculateIndexes(int[] redundantFields)
    {
        List<int> fields = datetimes.ToList();

        for (int index = 0; index < datetimes.Length; index++)
        {
            if (redundantFields.Contains(datetimes[index]))
            {
                fields.RemoveAt(index);
            }
        }

        for (int index = 0; index < datetimes.Length; index++)
        {
            datetimes[index] -= redundantFields.Where(f => f < datetimes[index]).Count();
        }

        return datetimes.ToArray();
    }
}
