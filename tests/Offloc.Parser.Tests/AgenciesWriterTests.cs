using Offloc.Parser.Writers;

namespace Offloc.Parser.Tests;

public class AgenciesWriterTests : IDisposable
{
    private readonly string _testDirectory;

    public AgenciesWriterTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"AgenciesWriterTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Theory]
    [InlineData("AGY001", "Agency One")]
    [InlineData("AGY002", "Agency Two")]
    [InlineData("AGY003", "Agency Three")]
    public async Task WriteAsync_WithSingleAgency_WritesAgency(string code, string name)
    {
        // Arrange
        var outputPath = Path.Combine(_testDirectory, Guid.NewGuid().ToString());
        Directory.CreateDirectory(outputPath);
        var writer = new AgenciesWriter(outputPath, []);

        var agency = new[]{ string.Empty, name, code };

        // Act
        await writer.WriteAsync("NOMS001", agency);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(outputPath, "Agencies.txt");
        Assert.True(File.Exists(outputFile));

        var lines = File.ReadAllLines(outputFile);
        
        Assert.Single(lines);
        Assert.Equal($"{code}|{name}", lines[0]);
    }

    [Fact]
    public async Task WriteAsync_WithMultipleAgencies_WritesAllAgencies()
    {
        // Arrange
        var writer = new AgenciesWriter(_testDirectory, []);
        
        var record1 = new[]{ string.Empty, "Agency One", "AGY001" };
        var record2 = new[]{ string.Empty, "Agency Two", "AGY002" };
        var record3 = new[]{ string.Empty, "Agency Three", "AGY003" };

        // Act
        await writer.WriteAsync("NOMS001", record1);
        await writer.WriteAsync("NOMS002", record2);
        await writer.WriteAsync("NOMS003", record3);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Agencies.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Equal(3, lines.Length);
        Assert.Equal("AGY001|Agency One", lines[0]);
        Assert.Equal("AGY002|Agency Two", lines[1]);
        Assert.Equal("AGY003|Agency Three", lines[2]);
    }

    [Fact]
    public async Task WriteAsync_WithDuplicateAgencyCodes_WritesNonDuplicates()
    {
        // Arrange
        var writer = new AgenciesWriter(_testDirectory, []);
        
        var record1 = new[]{ string.Empty, "Agency One", "AGY001" };
        var record2 = new[]{ string.Empty, "Agency Two", "AGY002" };
        var record3 = new[]{ string.Empty, "Agency One Duplicate", "AGY001" };

        // Act
        await writer.WriteAsync("NOMS001", record1);
        await writer.WriteAsync("NOMS002", record2);
        await writer.WriteAsync("NOMS003", record3);
        writer.Dispose();

        // Assert
        var outputFile = Path.Combine(_testDirectory, "Agencies.txt");
        var lines = await File.ReadAllLinesAsync(outputFile);
        
        Assert.Equal(2, lines.Length);
        Assert.Contains(lines, l => l.Contains("AGY001"));
        Assert.Contains(lines, l => l.Contains("AGY002"));
        Assert.DoesNotContain(lines, l => l.Contains("Duplicate"));
    }
    
    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
