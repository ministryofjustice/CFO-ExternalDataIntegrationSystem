
namespace Blocking.ConfigurationModels;

public class BlockingConfigurationDefaults
{
	public DatabaseTable offlocTable = new DatabaseTable
	{
		DatabaseName = "OfflocRunningPictureDb",
		TableName = "[OfflocRunningPicture].[RecordMatchingView]"
	};

	public DatabaseTable deliusTable = new DatabaseTable
	{
		DatabaseName = "DeliusRunningPictureDb",
		TableName = "[DeliusRunningPicture].[RecordMatchingView]"
	};
}
