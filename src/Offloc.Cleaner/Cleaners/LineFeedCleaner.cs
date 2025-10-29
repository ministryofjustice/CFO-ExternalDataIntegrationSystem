
using System.Globalization;
using System.Text;
using Serilog;

namespace Offloc.Cleaner.Cleaners;

public class FileCleaner
{
    private StreamWriter writer;
    private bool started;

    private string targetPath;
    private string newPath;

    private int[] redundantFields;
    private int ExpectedNumberOfFields => (153 - redundantFields.Length);

    public FileCleaner(string newPath, int[] redundantFields)
    {
        this.redundantFields = redundantFields;

        this.newPath = newPath;
        targetPath = newPath.Replace(".dat", "_clean.dat");

        Log.Information($"Reading '{newPath}' file into clean version '{targetPath}'");

        writer = File.CreateText(targetPath);

        writer.AutoFlush = false; //Should improve performance.
        writer.NewLine = "\r\n";
    }

    public string Clean()
    {
        using FileStream stream = new FileStream(newPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096);
        using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
        {
            string contents;
            while ((contents = reader.ReadLine()!) != null)
            {
                if (!ParseSystemDate(contents))
                {
                    continue;
                }

                //Below here is how the clean method was originally.                
                // cg fix the rogue pipe issue, we replace ,"|", with ,"",
                contents = contents.Replace(",\"|\",", ",\"\",");
                // cg fix end

                string[] split_contents = RemoveRedundantFields(contents);
                int count = split_contents.Count();
                contents = string.Join("\"|\"", split_contents);

                if (!CheckTusedDate(ref contents, count))
                {
                    continue;
                }
                writer.Write(contents);
            }
        }
        writer.Flush();
        writer.Close();
        return targetPath;
    }

    private bool ParseDate(string dateString)
    {
        DateTime d;
        return DateTime.TryParseExact(dateString, new[] { "dd/MM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out d);
    }

    private bool ParseSystemDate(string line)
    {
        // get the left most 12 characters, removes the speechmarks and then tries parsing as a datetime.
        if (line.Length >= 12)
        {
            var left = line[..12].Replace("\"", string.Empty);
            if (ParseDate(left))
            {
                // we have a valid line
                if (started)
                {
                    writer.WriteLine();
                }
                started = true;
                return true;
            }
            else
            {
                Log.Warning($"Unexpected value for datetime '{left}' on line: {line}");
                return false;
            }
        }
        else
        {
            Log.Warning($"Unexpected number of characters for datetime (expecting 12 or more, received {line?.Length ?? 0}) on Line: {line}");
            return false;
        }
    }

    //Passed by ref so original contents is mutated.
    private bool CheckTusedDate(ref string contents, int count)
    {
        //Pre-calculate compared value in future.
        if (count == ExpectedNumberOfFields - 1)
        {
            // if I am 152 I need to replace my contents end.
            var toTest = contents.Substring(contents.Length - 10);
            if (ParseDate(toTest))
            {
                //Final date isn't enclosed by speech marks.
                contents = contents.Substring(0, contents.Length - 10) + "\"" + contents.Substring(contents.Length - 10, 10) + "\"";
            }
            else
            {
                contents += "\"|\"\""; //Adds empty field at the end.
            }
            return true;
        }
        else if (count != 153 - redundantFields.Length)
        {
            Log.Warning($"Unexpected number of fields (expecting between 152 and 153, received {count}) on line: {contents}");
            return false;
        }
        else
        {
            return true;
        }
    }

    private string[] RemoveRedundantFields(string line)
    {
        List<string> split_line = line.Split("\"|\"", StringSplitOptions.None).ToList();

        if (split_line.Count-1 < redundantFields.Last())
        {
            return split_line.ToArray();
        }

        for (int i = (redundantFields.Length-1); i >= 0; i--)
        {
            split_line.RemoveAt(redundantFields[i]);
        }

        return split_line.ToArray();
    }
}