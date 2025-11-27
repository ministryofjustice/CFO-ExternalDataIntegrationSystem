using Offloc.Cleaner.Cleaners;

namespace Offloc.Cleaner.Tests;

public class FileCleanerTests : IDisposable
{
    private readonly string _testDirectory;

    public FileCleanerTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), $"FileCleanerTests_{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDirectory);
    }

    [Fact]
    public void Clean_CreatesCleanFile_WithCorrectNaming()
    {
        // Arrange
        var testFile = CreateTestFile("test.dat", CreateValidOfflocLine());

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        Assert.True(File.Exists(cleanedFile));
        Assert.Equal("test_clean.dat", Path.GetFileName(cleanedFile));
    }

    [Fact]
    public void Clean_WithValidLine_WritesCleanedLine()
    {
        // Arrange
        var testFile = CreateTestFile("test.dat", CreateValidOfflocLine());

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        Assert.True(File.Exists(cleanedFile));
        var content = File.ReadAllText(cleanedFile);
        Assert.NotEmpty(content);
    }

    [Fact]
    public void Clean_RemovesRoguePipeCharacters()
    {
        // Arrange
        var lineWithRoguePipe = "\"01/01/2024\"|\"Field1\",\"|\",\"Field2\"|\"Field3\"" + new string('|', 147);
        var testFile = CreateTestFile("test.dat", lineWithRoguePipe);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        Assert.DoesNotContain(",\"|\",", content);
    }

    [Fact]
    public void Clean_WithRedundantFields_RemovesFields()
    {
        // Arrange
        var line = CreateValidOfflocLineWithFieldCount(153);
        var testFile = CreateTestFile("test.dat", line);
        var redundantFields = new[] { 5, 10, 15 };

        var cleaner = new FileCleaner(testFile, redundantFields);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        Assert.True(File.Exists(cleanedFile));
        var content = File.ReadAllText(cleanedFile);
        var fieldCount = content.Split("\"|\"").Length;
        Assert.Equal(150, fieldCount);
    }

    [Fact]
    public void Clean_WithMultipleLines_ProcessesAllValidLines()
    {
        // Arrange
        var line1 = CreateValidOfflocLine();
        var line2 = CreateValidOfflocLineWithDate("02/01/2024");
        var line3 = CreateValidOfflocLineWithDate("03/01/2024");
        var testFile = CreateTestFile("test.dat", line1 + "\r\n" + line2 + "\r\n" + line3);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(3, lines.Length);
    }

    [Fact]
    public void Clean_WithInvalidDateLine_SkipsLine()
    {
        // Arrange
        var validLine = CreateValidOfflocLine();
        var invalidLine = "\"INVALID_DATE\"" + new string('|', 151);
        var testFile = CreateTestFile("test.dat", validLine + "\r\n" + invalidLine);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
    }

    [Fact]
    public void Clean_WithShortLine_SkipsLine()
    {
        // Arrange
        var validLine = CreateValidOfflocLine();
        var shortLine = "\"01/01\"";
        var testFile = CreateTestFile("test.dat", validLine + "\r\n" + shortLine);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        var lines = content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        Assert.Single(lines);
    }

    [Fact]
    public void Clean_With152Fields_AddsExtraFieldIfNeeded()
    {
        // Arrange
        var line = CreateValidOfflocLineWithFieldCount(152);
        var testFile = CreateTestFile("test.dat", line);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        Assert.NotEmpty(content);
    }

    [Fact]
    public void Clean_UsesCrlfLineEndings()
    {
        // Arrange
        var line1 = CreateValidOfflocLine();
        var line2 = CreateValidOfflocLineWithDate("02/01/2024");
        var testFile = CreateTestFile("test.dat", line1 + "\r\n" + line2);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        Assert.Contains("\r\n", content);
    }

    [Fact]
    public void Clean_WithEmptyFile_CreatesEmptyCleanFile()
    {
        // Arrange
        var testFile = CreateTestFile("test.dat", "");

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        Assert.True(File.Exists(cleanedFile));
        var content = File.ReadAllText(cleanedFile);
        Assert.Empty(content);
    }

    [Fact]
    public void Clean_WithSingleRedundantField_RemovesOneField()
    {
        // Arrange
        var line = CreateValidOfflocLineWithFieldCount(153);
        var testFile = CreateTestFile("test.dat", line);

        var cleaner = new FileCleaner(testFile, [150]);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        var fieldCount = content.Split("\"|\"").Length;
        Assert.Equal(152, fieldCount);
    }

    [Fact]
    public void Clean_WithMultipleRedundantFields_RemovesAllSpecified()
    {
        // Arrange
        var line = CreateValidOfflocLineWithFieldCount(153);
        var testFile = CreateTestFile("test.dat", line);
        var redundantFields = new[] { 1, 3, 5, 7, 9 };

        var cleaner = new FileCleaner(testFile, redundantFields);

        // Act
        var cleanedFile = cleaner.Clean();

        // Assert
        var content = File.ReadAllText(cleanedFile);
        var fieldCount = content.Split("\"|\"").Length;
        Assert.Equal(148, fieldCount);
    }

    private string CreateTestFile(string fileName, string content)
    {
        var filePath = Path.Combine(_testDirectory, fileName);
        File.WriteAllText(filePath, content);
        return filePath;
    }

    private string CreateValidOfflocLine()
    {
        return CreateValidOfflocLineWithFieldCount(153);
    }

    private string CreateValidOfflocLineWithFieldCount(int fieldCount)
    {
        return CreateValidOfflocLineWithDate("01/01/2024", fieldCount);
    }

    private string CreateValidOfflocLineWithDate(string date, int fieldCount = 153)
    {
        var fields = new List<string> { $"\"{date}\"" };
        for (int i = 1; i < fieldCount; i++)
        {
            fields.Add($"\"Field{i}\"");
        }
        return string.Join("|", fields);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
