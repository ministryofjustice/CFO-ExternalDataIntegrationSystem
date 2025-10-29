
using Delius.Parser.Configuration;
using Delius.Parser.Configuration.Models;
using Serilog;
using System.Text;

namespace Delius.Parser.Core;

public class DeliusOutputter
{
    #region private members
    private readonly Dictionary<string, StreamWriter> _writers = new Dictionary<string, StreamWriter>();
    private string _currentDocument = null;
    private bool isFirst = false;
    private long _offenderId;
    private StreamWriter currentWriter;
    private string? _path = null;
    #endregion

    public string Path
    {
        get
        {
            return _path ??= Environment.CurrentDirectory;
        }
        set
        {
            _path = value;
            Directory.CreateDirectory(_path);
        }
    }

    //Writers created to make SQL simpler (as it won't have to check for existence of file).
    private void InitializeWriters(string path)
    {
        _path = path;
        Directory.CreateDirectory(_path);
        //Path = Directory.CreateDirectory(path);

        Line[] lines = ConfigurationParser.GetLines();
        foreach (var line in lines.Where(l => l.OutputToFile))
        {
            var target = System.IO.Path.Combine(_path!, line.Name + ".txt");
            Log.Information($"Creating file '{target}'");
            FileStream fs = new FileStream(target, FileMode.Create, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.SequentialScan);
            var writer = new StreamWriter(fs, Encoding.Unicode);
            writer.NewLine = "\r\n";
            _writers.Add(line.Name, writer);
        }
    }

    public void StartOutput(string outputName, string path)
    {
        if (_writers.Count() == 0)
        {
            InitializeWriters(path);
        }

        _currentDocument = outputName;
        currentWriter = _writers[_currentDocument]; //A stream writer is created for each output file.
        isFirst = true;

    }

    public Task EndLine() => currentWriter.WriteLineAsync();

    public void Write(string value)
    {
        Log.Information("Write reached.");
        if (value == null)
        {
            WriteEmptyField();
        }
        else
        {
            WriteFieldTerminator();
            currentWriter.Write(value);
        }
    }

    public async Task WriteAsync(string value)
    {
        if (value == null)
        {
            await WriteEmptyFieldAsync();
        }
        else
        {
            WriteFieldTerminator();
            await currentWriter.WriteAsync(value);
        }
    }

    public void Write(long value)
    {
        Write(value.ToString());
    }

    public void Write(DateTime value)
    {
        if (value == default)
        {
            WriteEmptyField();
        }
        else
        {
            Write(value.ToString());
        }
    }

    public void Finish()
    {
        foreach (var item in _writers)
        {
            Log.Information($"Finalizing and saving changes to {item.Key}");
            item.Value.Flush();
            item.Value.Close();
            item.Value.Dispose();
        }
        _writers.Clear();
    }

    public void Dispose()
    {
        Finish();
    }

    private void WriteEmptyField()
    {
        WriteFieldTerminator();
    }

    private async Task WriteEmptyFieldAsync()
    {
        if (isFirst)
        {
            isFirst = false;
            await currentWriter.WriteAsync(_offenderId.ToString());
            await currentWriter.WriteAsync("|");
        }
        else
        {
            await currentWriter.WriteAsync("|");
        }
    }

    private void WriteFieldTerminator()
    {
        if (isFirst)
        {
            isFirst = false;
            currentWriter.Write(_offenderId);
            currentWriter.Write("|");
        }
        else
        {
            currentWriter.Write("|");
        }
    }

    public void SetOffenderId(long offenderId)
    {
        _offenderId = offenderId;
    }
}
