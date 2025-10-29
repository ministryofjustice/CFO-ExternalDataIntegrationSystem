CREATE TABLE [OfflocStaging].[PersonalDetails]
(
	[NOMSnumber] NVARCHAR(7) NOT NULL,
	[Forename1] NVARCHAR(50) NULL,
	[Forename2] NVARCHAR(50) NULL,
	[Surname] NVARCHAR(50) NULL,
	[DOB] DATE NOT NULL,
	[Gender] NVARCHAR(30) NOT NULL,
	[MaternityStatus] NVARCHAR(100) NULL,
	[Nationality] NVARCHAR(50) NULL,
	[Religion] NVARCHAR(50) NULL,
	[MaritalStatus] NVARCHAR(50) NULL,
	[EthnicGroup] NVARCHAR(50) NULL,
)ON [PRIMARY]
