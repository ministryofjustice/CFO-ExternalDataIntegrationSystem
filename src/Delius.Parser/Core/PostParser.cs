
using Delius.Parser.PostParsingConfig;
using System.Security.Cryptography;
using System.Text;

namespace Delius.Parser.Core;

//Class used to split up tables derived from the input file.
public class PostParser
{
    private PostParsingConfiguration[] postParsingConfigurations;

    private StreamReader baseFileReader;
    private Dictionary<string, StreamWriter> NewFileWriters = new Dictionary<string, StreamWriter>();
    private Dictionary<string, HashSet<string>> NewFilePreviousLines = new Dictionary<string, HashSet<string>>();
    private StreamWriter[] newFileWriters;

    private string outputFolderPath = string.Empty;
    private string lastHash = string.Empty;

    public PostParser()
    {
        postParsingConfigurations = PostParsingConfigurationParser.GetPostParsingConfigurations();
    }

    private void Initialization(string outputFolderPath)
    {
        Console.WriteLine(outputFolderPath);

        this.outputFolderPath = outputFolderPath;
        lastHash = string.Empty;

        NewFileWriters.Clear();
        NewFilePreviousLines.Clear();
        newFileWriters = Array.Empty<StreamWriter>();
    }

    //Performs post-parsing according to post-parsing configurations.
    public async Task PostParse(string outputFolderPath)
    {
        Initialization(outputFolderPath);

        var groupedConfigs = postParsingConfigurations.GroupBy(c => c.BaseFileName);

        //Done this way so you only have to go over an input file once.
        foreach (var config in groupedConfigs)
        {
            string baseFileName = config.First().BaseFileName;

			var linkedConfigs = config.Where(c => baseFileName == config.Key).ToArray();
			InitializeWriters(linkedConfigs);

			await PostParseFile(linkedConfigs);

            DisposeStreams();

            var oldFile = Path.Combine(outputFolderPath, $"{baseFileName}.txt");
            var newFile = Path.Combine(outputFolderPath, $"{baseFileName}_new.txt");
            
            if (File.Exists(newFile))
            {
                File.Delete(oldFile);
                File.Move(newFile, oldFile);
            }
            else
            {
				//Deletes original file afterwards.
                File.Delete(oldFile);
			}
        }
    } 

    private void DisposeStreams()
    {
        baseFileReader.Dispose();
        foreach (var writer in newFileWriters)
        {
            writer.Dispose();
        }
    }
    
    private async Task PostParseFile(PostParsingConfiguration[] postParsingConfigs)
    {
        //Process non-hashing first so hashed value can be appended to end.
        var nonHashingPostParserConfigs = postParsingConfigs.Where(c => c.GenerateCompositeHash == false).ToArray();
        var hashingPostParserConfigs = postParsingConfigs.Where(c => c.GenerateCompositeHash == true).ToArray();

        string? line;

        using (baseFileReader)
        {
			while (!string.IsNullOrEmpty(line = await baseFileReader.ReadLineAsync()!))
			{
				await WriteLineForGroup(hashingPostParserConfigs, line);
				await WriteLineForGroup(nonHashingPostParserConfigs, line);
                
			}
		}
    }

    private async Task WriteLineForGroup(PostParsingConfiguration[] configs,  string line)
    {
	    string[] splitLine = line.Split('|');

		for (int i = 0; i < configs.Length; i++)
		{
			var writerLines = NewFilePreviousLines[configs[i].NewFileName];
            var newFileWriter = NewFileWriters[configs[i].NewFileName];

			splitLine = CleanLine(splitLine);
			//Fixes edge case where there are duplicate building names with different casings.
			splitLine = NormalizeFields(splitLine, configs[i].FieldsToNormalize);

			string newLine = JoinNewFields(splitLine, configs[i]);

			if (configs[i].GenerateCompositeHash)
			{
				newLine = string.Join('|', ComputeHash(newLine.Split('|')));

                lastHash = newLine.Split('|').First();
			}

            if (configs[i].HashSourceTable != string.Empty)
            {
                newLine += $"|{lastHash}";
            }

			if (!writerLines.Contains(newLine.ToLower())) //Prevents duplicates.
			{
				await newFileWriter.WriteLineAsync(newLine);

				writerLines.Add(newLine.ToLower());
			}
		}
	}

    private void InitializeWriters(PostParsingConfiguration[] postParsingConfigs)
    {
        string baseFile = $"{outputFolderPath}/{postParsingConfigs[0].BaseFileName}.txt";

        baseFileReader = new StreamReader(baseFile);
        //newBaseFileWriter = new StreamWriter($"{outputFolderPath}/{postParsingConfigs[0].BaseFileName}_new.txt");

        newFileWriters = new StreamWriter[postParsingConfigs.Length];
        for (int i = 0; i<postParsingConfigs.Length; i++)
        {
            //Handle edge case for base file and new file name being the same.
            if (postParsingConfigs[i].NewFileName == postParsingConfigs[i].BaseFileName)
            {
                postParsingConfigs[i].NewFileName += "_new";
            }

            newFileWriters[i] = new StreamWriter($"{outputFolderPath}/{postParsingConfigs[i].NewFileName}.txt", append: false, encoding: Encoding.Unicode);
            newFileWriters[i].NewLine = "\r\n";
            
            NewFileWriters.Add(postParsingConfigs[i].NewFileName, newFileWriters[i]);
            NewFilePreviousLines.Add(postParsingConfigs[i].NewFileName, new HashSet<string>(2500));
        }
	}

    private string[] NormalizeFields(string[] splitLine, int[] fieldsToNormalize)
    {
        foreach(var fieldIndex in fieldsToNormalize) 
        {
            DateTime dt;
            if (DateTime.TryParse(splitLine[fieldIndex], out dt))
            {
                DateOnly dOnly = DateOnly.FromDateTime(dt);
                splitLine[fieldIndex] = dOnly.ToString();
            }
            else
            {
                string field = splitLine[fieldIndex];
                string[] words = field.Split(' ');

                for (int i = 0; i < words.Length; i++)
                {
                    if (string.IsNullOrEmpty(words[i]))
                    {
                        continue;
                    }

                    words[i] = words[i].ToLower();

                    string firstChar = words[i].ElementAt(0).ToString().ToUpper();

                    words[i] = $"{firstChar}{words[i][1..]}";
                }

                splitLine[fieldIndex] = string.Join(' ', words);
            }
        }

        return splitLine;
    }

    private string JoinNewFields(string[] splitLine, PostParsingConfiguration config)
    {
        return JoinRelevantFields(splitLine, config.RelevantFields);
    }

    private string JoinRelevantFields(string[] splitLine, int[] indexes)
    {
        string[] fields = new string[indexes.Length];

        for (int i = 0; i < indexes.Length; i++)
        {
            fields[i] = splitLine[indexes[i]].Trim();
        }

		return string.Join('|', fields).Trim();
	}

    private string[] CleanLine(string[] splitLine)
    {
        for (short i = 0; i<splitLine.Length; i++)
        {
            splitLine[i] = splitLine[i].Trim();
        }

        return splitLine;
    }

    //Returns a split string with the compute hash appended to the end.
    private string[] ComputeHash(string[] splitLine)
    {
        string str = string.Join('|', splitLine);

        byte[] bytes = ASCIIEncoding.Unicode.GetBytes(str); 

        var hash = SHA256.HashData(bytes);

        string[] newSplitLine = new string[splitLine.Length + 1];
        
        Array.Copy(splitLine, 0, newSplitLine, 1, splitLine.Length);

        System.Text.StringBuilder s = new System.Text.StringBuilder();
		foreach (byte b in hash)
		{
			s.Append(b.ToString("x2").ToLower());
		}

        newSplitLine[0] = s.ToString();

        return newSplitLine;
	}
}