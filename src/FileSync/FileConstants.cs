public static class FileConstants
{
    public const string DeliusFilePattern = @"^cfoextract_\d{1,5}_(full|diff)_\d{14}\.txt$";
    public const string OfflocFilePattern = @"^C_NOMIS_OFFENDER_\d{8}_.+\.dat$";
    public const string OfflocArchivePattern = @"^\d{8}\.zip$";
    public const string OfflocFileOrArchivePattern = @"^(?:\d{8}\.zip|C_NOMIS_OFFENDER_\d{8}_.+\.dat)$";
}