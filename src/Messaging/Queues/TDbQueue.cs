
namespace Messaging.Queues;

public enum TDbQueue
{
    //These queues will use Rabbit RPC.

    //Outgoing requests.
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
    ResultClearOffloc
}
