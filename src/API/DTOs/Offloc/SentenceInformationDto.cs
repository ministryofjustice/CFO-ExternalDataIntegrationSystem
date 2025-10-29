namespace API.DTOs.Offloc
{
    public class SentenceInformationDto
    {
        public DateOnly? FirstSentenced { get; set; }

        public int? SentenceYears { get; set; }

        public int? SentenceMonths { get; set; }

        public int? SentenceDays { get; set; }

        public DateOnly? EarliestPossibleReleaseDate { get; set; }

        public DateOnly? DateOfRelease { get; set; }

        public string? Sed { get; set; }

        public string? Hdced { get; set; }

        public string? Hdcad { get; set; }

        public string? Ped { get; set; }

        public string? Crd { get; set; }

        public string? Npd { get; set; }

        public string? Led { get; set; }

        public DateOnly? Tused { get; set; }

        public bool IsActive { get; set; }

    }
}
