namespace DbInteractions.Services;

public record FailedStagingRow(
    DateTime Timestamp,
    string FileName,
    string FilePath,
    string TableReference,
    string FailureType,
    int? LineNumber,
    string RowContent,
    string ErrorMessage,
    string? ErrorDetail
);
