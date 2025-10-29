
namespace Blocking.ConfigurationModels;

//Necessary to be able to match between different tables.
public class BlockingFieldsGroup
{
	public BlockingField[] BlockingFields { get; init; } = Array.Empty<BlockingField>();
	//These could also be views
	public DatabaseTable OfflocTable { get; init; } = new DatabaseTable //Default.
	{
		DatabaseName = "OfflocRunningPictureDb",
		TableName = "[OfflocRunningPicture].[RecordMatchingView]"
	};
	public DatabaseTable DeliusTable { get; init; } = new DatabaseTable //Default.
	{
		DatabaseName = "DeliusRunningPictureDb",
		TableName = "[DeliusRunningPicture].[RecordMatchingView]"
	};
}
