namespace Messaging.Queues;

public enum TStagingQueue
{
    Kickoff,
    DeliusFilesClear,
    DeliusFileDownload,
    DeliusParser,
    DeliusImport,
    DeliusCleanup,
    OfflocFilesClear,
    OfflocFileDownload,
    OfflocCleaner,
    OfflocParser,
    OfflocImport,
    OfflocCleanup,
    StatusUpdate
}
