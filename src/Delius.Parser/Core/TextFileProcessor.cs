﻿using System.Text;
using Messaging.Interfaces;
using Messaging.Messages.StagingMessages;
using Messaging.Queues;
using Serilog;

namespace Delius.Parser.Core;

public class TextFileProcessor : IFileProcessor
{
    private readonly IStagingMessagingService messageService;
    private DeliusProcessor deliusProcessor;

    public TextFileProcessor(IStagingMessagingService messageService, DeliusProcessor deliusProcessor)
    {
        this.messageService = messageService;
        this.deliusProcessor = deliusProcessor;

        //messageService.StagingSubscribe<DeliusDownloadFinishedMessage>(async(message) => await Process(), TStagingQueue.DeliusParser);
    }

    public async Task Process(string fileToBeParsed, string outputDirectory)
    {
        //DeliusOutputter outputter = new DeliusOutputter(outputDirectory);
        StreamReader sr = new StreamReader(fileToBeParsed, Encoding.UTF8, false, 4096);

        await deliusProcessor.Process(sr, outputDirectory, UnhandledLine);
    }

    private static void UnhandledLine(string line)
    {
        Log.Warning($"Unhandled line of {line?.Length ?? 0} characters: {line}");
    }
}
