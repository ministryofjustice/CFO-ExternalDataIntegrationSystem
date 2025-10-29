namespace Offloc.Parser.Writers;

/// <summary>
/// Represents a type that takes a string array and outputs it to a file.
/// Used by the offloc processor to split the OffLoc file into it's distinct types and parts
/// </summary>
public interface IWriter : IDisposable
{
    Task WriteAsync(string NOMSNumber, string[] contents);
}