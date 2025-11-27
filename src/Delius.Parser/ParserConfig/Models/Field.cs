
using System.Globalization;

namespace Delius.Parser.Configuration.Models;

public class Field
{
    public int LineId { get; set; }
    public int Id { get; set; }

    public string Name { get; set; }
    public int Length { get; set; }
    public int StartingPoint { get; set; }
    public FieldType Type { get; set; }

    public virtual Line Line { get; set; }

    private static CultureInfo cultureInfo = new CultureInfo("en-GB");


	public string Parse(string text)
    {
        switch (Type)
        {
            case FieldType.String:
                return text.Substring(StartingPoint, Length).Replace("~", "");
            case FieldType.Long:
                string l = text.Substring(StartingPoint, Length).Replace("~", "");
                if (string.IsNullOrWhiteSpace(l))
                {
                    return "0";
                }
                else
                {
                    return l;
                }
            case FieldType.LongDate:
                //dates :)
                string d = text.Substring(StartingPoint, Length).Replace("~", "");
                if (string.IsNullOrWhiteSpace(d))
                {
                    return string.Empty;
                }
                else
                {
                    var date = ParseDatetime(d);
                    return date.ToString(cultureInfo);
                }
            case FieldType.ShortDate:
                string sd = text.Substring(StartingPoint, Length).Replace("~", "");
                if (string.IsNullOrWhiteSpace(sd))
                {
                    return string.Empty;
                }
                else
                {
                    var date = ParseDate(sd);
                    return date.ToString(cultureInfo);
                }
        }
        throw new ApplicationException("Unknown field type: " + Type);
    }

    //Dates are in the format YYYYMMDD with no delimitation.
    private DateTime ParseDatetime(string dateString)
    {
        if (dateString.Length > 14)
        {
            dateString = dateString.Substring(0, 8);
        }

        dateString = $"{dateString[6..8]}/{dateString[4..6]}/{dateString[..4]}";

        DateTime.TryParseExact(dateString, new[] { "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate);

        return parsedDate;
    }

    //Treats long and short dates the same as 
    private DateOnly ParseDate(string dateString)
    {
        if (dateString.Length != 8)
        {
            throw new ApplicationException($"Date string {dateString} was not the expected length of 8.");
        }
        return DateOnly.FromDateTime(ParseDatetime(dateString));
    }
}
