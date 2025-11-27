using Offloc.Parser.Services.TrimmerContext.SecondaryContexts;
using Offloc.Parser.Writers.GroupWriters.Address;

namespace Offloc.Parser.Tests;

public class AddressWriterTests : IDisposable
{
    private readonly string _testDirectory;

    public AddressWriterTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"AddressWriterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public async Task WriteAsync_WithAllAddressTypes_WritesAllAddresses()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateFullRecordArray();

        // Act
        await writer.WriteAsync("NOMS001", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Equal(5, lines.Length);
        Assert.Contains(lines, l => l.StartsWith("NOMS001|Discharge|"));
        Assert.Contains(lines, l => l.StartsWith("NOMS001|Reception|"));
        Assert.Contains(lines, l => l.StartsWith("NOMS001|Home|"));
        Assert.Contains(lines, l => l.StartsWith("NOMS001|NOK|"));
        Assert.Contains(lines, l => l.StartsWith("NOMS001|Probation|"));
    }

    [Fact]
    public async Task WriteAsync_WithDischargeAddressOnly_WritesDischargeOnly()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateRecordWithDischargeOnly();

        // Act
        await writer.WriteAsync("NOMS002", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Single(lines);
        Assert.StartsWith("NOMS002|Discharge|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithReceptionAddressOnly_WritesReceptionOnly()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateRecordWithReceptionOnly();

        // Act
        await writer.WriteAsync("NOMS003", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Single(lines);
        Assert.StartsWith("NOMS003|Reception|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithHomeAddressOnly_WritesHomeOnly()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateRecordWithHomeOnly();

        // Act
        await writer.WriteAsync("NOMS004", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Single(lines);
        Assert.StartsWith("NOMS004|Home|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithNOKAddressOnly_WritesNOKOnly()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateRecordWithNOKOnly();

        // Act
        await writer.WriteAsync("NOMS005", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Single(lines);
        Assert.StartsWith("NOMS005|NOK|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithProbationAddressOnly_WritesProbationOnly()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateRecordWithProbationOnly();

        // Act
        await writer.WriteAsync("NOMS006", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Single(lines);
        Assert.StartsWith("NOMS006|Probation|", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithEmptyAddresses_CreatesEmptyFile()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = CreateRecordWithNoAddresses();

        // Act
        await writer.WriteAsync("NOMS007", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        Assert.True(File.Exists(outputFile));
        var lines = await File.ReadAllLinesAsync(outputFile);
        Assert.Empty(lines);
    }

    [Fact]
    public async Task WriteAsync_WithMultipleRecords_WritesAllRecords()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents1 = CreateRecordWithDischargeOnly();
        var contents2 = CreateRecordWithReceptionOnly();
        var contents3 = CreateRecordWithHomeOnly();

        // Act
        await writer.WriteAsync("NOMS008", contents1);
        await writer.WriteAsync("NOMS009", contents2);
        await writer.WriteAsync("NOMS010", contents3);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Equal(3, lines.Length);
        Assert.StartsWith("NOMS008|Discharge|", lines[0]);
        Assert.StartsWith("NOMS009|Reception|", lines[1]);
        Assert.StartsWith("NOMS010|Home|", lines[2]);
    }

    [Fact]
    public async Task WriteAsync_CorrectlyFormatsDischargeAddress()
    {
        // Arrange
        var context = new AddressFieldsContext([]);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = new string[117];
        Array.Fill(contents, "");
        
        contents[77] = "123 Main St";
        contents[78] = "Apt 4";
        contents[79] = "London";
        contents[80] = "Greater London";
        contents[81] = "UK";
        contents[82] = "SW1A 1AA";
        contents[83] = "020-1234-5678";
        contents[84] = "Notes";

        // Act
        await writer.WriteAsync("NOMS011", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Single(lines);
        Assert.Equal("NOMS011|Discharge||123 Main St|Apt 4|London|Greater London|UK|SW1A 1AA|020-1234-5678|Notes", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithRedundantFields_AdjustsIndices()
    {
        // Arrange
        var redundantFields = new[] { 0, 10, 20, 30, 40, 50, 60, 70 };
        var context = new AddressFieldsContext(redundantFields);
        var writer = new AddressWriter(_testDirectory, context);
        
        var contents = new string[109];
        Array.Fill(contents, "");
        
        contents[69] = "Test Street";

        // Act
        await writer.WriteAsync("NOMS012", contents);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Addresses.txt");
        Assert.True(File.Exists(outputFile));
    }

    private string[] CreateFullRecordArray()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        
        contents[77] = "Discharge St";
        contents[85] = "Reception Ave";
        contents[93] = "Home Rd";
        contents[100] = "NOK Ln";
        contents[110] = "Probation Blvd";
        
        return contents;
    }

    private string[] CreateRecordWithDischargeOnly()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        contents[77] = "123 Discharge Street";
        return contents;
    }

    private string[] CreateRecordWithReceptionOnly()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        contents[85] = "456 Reception Avenue";
        return contents;
    }

    private string[] CreateRecordWithHomeOnly()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        contents[93] = "789 Home Road";
        return contents;
    }

    private string[] CreateRecordWithNOKOnly()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        contents[100] = "321 NOK Lane";
        return contents;
    }

    private string[] CreateRecordWithProbationOnly()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        contents[110] = "654 Probation Boulevard";
        return contents;
    }

    private string[] CreateRecordWithNoAddresses()
    {
        var contents = new string[117];
        Array.Fill(contents, "");
        return contents;
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            try
            {
                Directory.Delete(_testDirectory, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }
}
