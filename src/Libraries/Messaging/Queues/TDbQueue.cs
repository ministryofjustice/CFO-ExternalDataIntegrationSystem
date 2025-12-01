
namespace Messaging.Queues;

public enum TDbQueue
{
    //These queues will use Rabbit RPC.

    //Outgoing requests.
    AssociateOfflocFileWithArchive,
    GetProcessedOfflocFiles,
    GetProcessedDeliusFiles,
    StageOffloc,
    StageDelius,
    MergeOffloc,
    MergeDelius,
    ClearOfflocStaging,
    ClearDeliusStaging,
    OfflocFileProcessingStarted,
    DeliusFileProcessingStarted,
    IsDeliusReadyForProcessing,
    IsOfflocReadyForProcessing,
    GetLastProcessedDeliusFile,
    GetLastProcessedOfflocFile,
    //Incoming responses.
    ReturnedOfflocFiles,
    ReturnedDeliusFiles,
    ResultStageOffloc,
    ResultStageDelius,
    ResultMergeOffloc,
    ResultMergeDelius,
    ResultClearDelius,
    ResultClearOffloc,
    ResultOfflocFileProcessingStarted,
    ResultDeliusFileProcessingStarted,
    ResultAssociateOfflocFileWithArchive,
    IsDeliusReadyForProcessingResult,
    IsOfflocReadyForProcessingResult,
    ResultLastProcessedDeliusFile,
    ResultLastProcessedOfflocFile
}
