
namespace Delius.Parser.Configuration.Models;

public class Line
{
    public List<Field> Fields = new List<Field>();

    public int Id { get; set; }
    public string Name { get; set; }
    public int Length { get; set; }
    public string StartingKey { get; set; }
    public string ParentKey { get; set; }

    public bool AllowSplit { get; set; }

    public bool OutputToFile { get; set; }

    public bool OutputToLog { get; set; }

    public string[] Split(string text)
    {
        string[] content = new string[Fields.Count];
        int index = 0;
        foreach (var field in Fields.OrderBy(r => r.StartingPoint))
        {
            try
            {
                content[index] = field.Parse(text).Replace("|", " ");
                index++;
            }
            catch (Exception ex)
            {
                string error = "Error reading field " + field.Name + " for " + Name + ". Message was: " + ex.Message + Environment.NewLine + "Original Line: " + text;

                throw new ApplicationException(error, ex);
            }
        }
        return content;
    }

    //For testing only.
    public override string ToString()
    {
        return Id + "- " + Name + "\n";
    }
}
