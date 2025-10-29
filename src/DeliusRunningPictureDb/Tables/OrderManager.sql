CREATE TABLE [dbo].[OrderManager](
	[OffenderId] [bigint] NOT NULL,
	[Id] [bigint] NOT NULL,
	[EventId] [bigint] NOT NULL,
	[CrcCode] [nvarchar](3) NULL,
	[CrcDescription] [nvarchar](60) NULL,
	[AllocationDate] [datetime] NULL,
	[ReasonCode] [nvarchar](100) NULL,
	[ReasonDescription] [nvarchar](500) NULL,
	[Deleted] [nvarchar](1) NULL,
	[LastUpdated] [datetime] NULL
) ON [PRIMARY]