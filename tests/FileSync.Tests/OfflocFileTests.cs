using FileSync.Extensions;

namespace FileSync.Tests;

public class OfflocFileTests
{
    [Fact]
    public void GetIsArchive_OfflocFile_ReturnsFalse()
    {
        // Arrange - Format: C_NOMIS_OFFENDER_ddMMyyyy_ID.dat
        var offlocFile = new OfflocFile("C_NOMIS_OFFENDER_01012024_01.dat");

        // Act
        var isArchive = offlocFile.IsArchive;

        // Assert
        Assert.False(isArchive);
    }

    [Fact]
    public void GetIsArchive_OfflocArchive_ReturnsTrue()
    {
        // Arrange - Format: yyyyMMdd.zip
        var offlocFile = new OfflocFile("20240101.zip");

        // Act
        var isArchive = offlocFile.IsArchive;

        // Assert
        Assert.True(isArchive);
    }

    [Fact]
    public void GetFileId_OfflocFile_ReturnsCorrectId()
    {
        // Arrange - Format: C_NOMIS_OFFENDER_ddMMyyyy_ID.dat
        var offlocFile = new OfflocFile("C_NOMIS_OFFENDER_01012024_01.dat");

        // Act
        var fileId = offlocFile.GetFileId();

        // Assert
        Assert.Equal(1012024, fileId);
    }

    [Fact]
    public void GetFileId_OfflocArchive_ReturnsNull()
    {
        // Arrange - Format: yyyyMMdd.zip
        var offlocFile = new OfflocFile("20240101.zip");

        // Act
        var fileId = offlocFile.GetFileId();

        // Assert
        Assert.Null(fileId);
    }

    [Fact]
    public void GetDatestamp_OfflocFile_ReturnsCorrectDate()
    {
        // Arrange - Format: C_NOMIS_OFFENDER_ddMMyyyy_ID.dat
        var offlocFile = new OfflocFile("C_NOMIS_OFFENDER_15012024_01.dat");

        // Act
        var datestamp = offlocFile.GetDatestamp();

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 15), datestamp);
    }

    [Fact]
    public void GetDatestamp_OfflocArchive_ReturnsCorrectDate()
    {
        // Arrange
        var offlocFile = new OfflocFile("20240115.zip");

        // Act
        var datestamp = offlocFile.GetDatestamp();

        // Assert
        Assert.Equal(new DateOnly(2024, 1, 15), datestamp);
    }
}