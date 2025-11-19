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
            return DateOnly.ParseExact(id.ToString()!, "ddMMyyyy", CultureInfo.InvariantCulture);
        }
    }

}