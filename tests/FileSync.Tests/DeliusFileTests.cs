using FileSync.Extensions;

namespace FileSync.Tests;

public class DeliusFileTests
{
    [Fact]
    public void GetFileId_DeliusFull_ReturnsCorrectId()
    {
        // Arrange
        var deliusFile = new DeliusFile("cfoextract_0123_full_20240101153000.txt");

        // Act
        var fileId = deliusFile.GetFileId();

        // Assert
        Assert.Equal("0123", fileId);
    }

    [Fact]
    public void GetFileId_DeliusDiff_ReturnsCorrectId()
    {
        // Arrange
        var deliusFile = new DeliusFile("cfoextract_0123_diff_20240101153000.txt");

        // Act
        var fileId = deliusFile.GetFileId();

        // Assert
        Assert.Equal("0123", fileId);
    }

    [Fact]
    public void GetDatestamp_DeliusFull_ReturnsCorrectDate()
    {
        // Arrange
        var deliusFile = new DeliusFile("cfoextract_0123_full_20240101153000.txt");

        // Act
        var datestamp = deliusFile.GetDatestamp();

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 1), datestamp);
    }

    [Fact]
    public void GetDatestamp_DeliusDiff_ReturnsCorrectDate()
    {
        // Arrange
        var deliusFile = new DeliusFile("cfoextract_0123_diff_20240101153000.txt");

        // Act
        var datestamp = deliusFile.GetDatestamp();

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 1), datestamp);
    }

    [Theory]
    [InlineData("cfoextract_0123_full_20241231153000.txt", 2024, 12, 31)]
    [InlineData("cfoextract_0123_full_20240101153000.txt", 2024, 1, 1)]
    [InlineData("cfoextract_0123_full_20240615153000.txt", 2024, 6, 15)]
    public void GetDatestamp_DeliusFile_VariousDates_ReturnsCorrectDate(
        string fileName, int year, int month, int day)
    {
        // Arrange
        var deliusFile = new DeliusFile(fileName);

        // Act
        var datestamp = deliusFile.GetDatestamp();

        // Assert
        Assert.Equal(new DateOnly(year, month, day), datestamp);
    }
}