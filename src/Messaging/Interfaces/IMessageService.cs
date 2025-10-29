namespace Messaging.Interfaces;

//This interface is defined assuming each queue will only have 1 type of messages.
//Perhaps should be split up into mutliple pub/sub interfaces.
public interface IMessageService : IStagingMessagingService, IMergingMessagingService, IStatusMessagingService, 
    IDbMessagingService, IImportMessagingService, IBlockingMessagingService, IMatchingMessagingService
{ }
