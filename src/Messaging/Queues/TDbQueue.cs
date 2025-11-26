
namespace Messaging.Queues;

public enum TDbQueue
{
    //These queues will use Rabbit RPC.

    //Outgoing requests.
    AssociateOfflocFileWithArchive,
    GetProcessedOfflocFiles,
    GetOfflocFileDates,
    DeliusGetLastFullId,
    GetProcessedDeliusFiles,
    StageOffloc,
    StageDelius,
    MergeOffloc,
    MergeDelius,
    StandardiseDelius,
    ClearOfflocStaging,
    ClearDeliusStaging,
    OfflocFileProcessingStarted,
    DeliusFileProcessingStarted,
    IsDeliusReadyForProcessing,
    IsOfflocReadyForProcessing,
    GetLastProcessedDeliusFile,
    GetLastProcessedOfflocFile,
    //Incoming requests.
    ReturnedOfflocFiles,
    ReturnedOfflocFileDates,
    ReturnDeliusGetLastFull,
    ReturnedDeliusFiles,
    ResultStageOffloc,
    ResultStageDelius,
    ResultMergeOffloc,
    ResultMergeDelius,
    ResultStandardiseDelius,
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
