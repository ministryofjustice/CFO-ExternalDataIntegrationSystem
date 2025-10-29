using System.Text;
using Serilog;

namespace Offloc.Parser.Writers;

public abstract class WriterBase 
{
    private bool disposedValue; 
    
    protected StreamWriter? StreamWriter { get; private set; }
    
    protected WriterBase(string path)
    {
        Log.Information($"{GetType().Name} is writing to file '{path}'");
        StreamWriter = new StreamWriter(path, false, Encoding.Unicode);
        StreamWriter.NewLine = "\r\n"; //So mssql can read the files.
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue == false)
        {
            if (disposing)
            {
                StreamWriter?.Flush();
                StreamWriter?.Close();
                StreamWriter?.Dispose();
            }

            StreamWriter = null;
            disposedValue = true;
        }
    }

    ~WriterBase()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}