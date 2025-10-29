
using Delius.Parser.Configuration.Models;
using System.Xml.Serialization;

namespace Delius.Parser.PostParsingConfig;

public static class PostParsingConfigurationParser
{

    public static PostParsingConfiguration[] GetPostParsingConfigurations()
    {
        var ser = new XmlSerializer(typeof(PostParsingConfiguration[]));

        var streamReader = new StreamReader(AppContext.BaseDirectory + "/PostParsingConfig/DeliusPostParsingConfig.xml");

        var lines = ser.Deserialize(streamReader) as PostParsingConfiguration[] ?? throw new ApplicationException("Cannot deserialize the file :(");
        
        return lines;
    }
}
