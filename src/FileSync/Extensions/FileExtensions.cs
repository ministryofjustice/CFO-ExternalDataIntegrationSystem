using System.Globalization;

namespace FileSync.Extensions;

public static class FileExtensions
{
    public static string GetFileId(this DeliusFile file)
    {
        var parts = file.Name.Split('_', StringSplitOptions.RemoveEmptyEntries);
        return parts[1];
    }

    public static int? GetFileId(this OfflocFile file)
    {
        if (file.IsArchive)
        {
            return null;
        }

        var parts = file.Name.Split('_', StringSplitOptions.RemoveEmptyEntries);
        return int.Parse(parts[3]);
    }

    public static DateOnly GetDatestamp(this DeliusFile file)
    {
        var parts = file.Name.Split('_', StringSplitOptions.RemoveEmptyEntries);
        var datePart = parts.Last().Substring(0, 8);
        return DateOnly.ParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture);
    }

    public static DateOnly GetDatestamp(this OfflocFile file)
    {
        if (file.IsArchive)
        {
            var datePart = Path.GetFileNameWithoutExtension(file.Name);
            return DateOnly.ParseExact(datePart, "yyyyMMdd", CultureInfo.InvariantCulture);
        }
        else
        {
            var id = file.GetFileId();
            var datePart = id.ToString()!.PadLeft(8, '0'); // 01/01/2024 becomes 1/01/2025, add leading zero to mitigate
            return DateOnly.ParseExact(datePart, "ddMMyyyy", CultureInfo.InvariantCulture);
        }
    }

}