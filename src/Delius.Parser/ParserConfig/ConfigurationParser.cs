
using System.Xml.Serialization;
using Delius.Parser.Configuration.Models;

namespace Delius.Parser.Configuration;

//Parses DeliusConfiguration.xml in same directory to construct a list of lines that can then be used by the processor.
//Assumes structure and type of xml file is correct and simply copies lines in xml file to Line and Field instances.
public static class ConfigurationParser
{
    internal static Line[] GetLines()
    {
        var ser = new XmlSerializer(typeof(Line[]));

        var streamReader = new StreamReader(AppContext.BaseDirectory + "/ParserConfig/DeliusConfiguration.xml");

        var lines = ser.Deserialize(streamReader) as Line[] ?? throw new ApplicationException("Cannot deserialize the file :(");
        return lines;

    }
}
