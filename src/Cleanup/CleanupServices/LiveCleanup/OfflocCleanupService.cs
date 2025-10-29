using FileStorage;
using Messaging.Interfaces;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;

namespace Cleanup.CleanupServices.LiveCleanup;

public class OfflocCleanupService : CleanupServiceBase
{
	private readonly IDbMessagingService dbMessagingService;

    public OfflocCleanupService(IFileLocations fileLocations, IDbMessagingService dbMessagingService)
	: base(fileLocations.offlocInput, fileLocations.offlocOutput)
	{
		this.dbMessagingService = dbMessagingService;
	}

	//A method to make DMS more resilient (by stopping it from trying to process half-cleaned files).
	public override void ClearIllegalFiles()
	{
		DirectoryInfo di = new DirectoryInfo(inputFolderPath);
		var illegalFiles = di.GetFiles("*_clean*");

		foreach (var illegalFile in illegalFiles) 
		{ 
			File.Delete(illegalFile.FullName);
		}
	}
}
