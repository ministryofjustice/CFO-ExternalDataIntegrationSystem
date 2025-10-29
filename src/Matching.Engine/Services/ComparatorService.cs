﻿using Messaging.Interfaces;
using Messaging.Messages.BlockingFinishedMessages;
using Messaging.Messages.MatchingMessages;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;
using System.Collections.Immutable;

namespace Matching.Engine.Services;

public class ComparatorService(
    ILogger<ComparatorService> logger,
    IOptions<List<MatchingOption>> options,
    IStatusMessagingService statusMessagingService,
    IBlockingMessagingService blockingMessagingService,
    IMatchingMessagingService matchingMessagingService,
    IMatchingRepository matchingRepository,
    IClusteringRepository clusteringRepository,
    MatcherCache cache,
    MatchingQueue queue) : BackgroundService
{
    private Dictionary<string, MatchingOption> matchingOptions = [];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await Task.Run(() =>
            {
                matchingOptions = options.Value.ToDictionary(o => o.MatchingKey);

                blockingMessagingService.BlockingSubscribe<BlockingFinishedMessage>(async (message) =>
                {
                    var items = await matchingRepository.GetAllAsync();

                    var records = items
                        .Cast<IDictionary<string, object>>()
                        .ToImmutableArray();

                    await ProcessAsync(records, stoppingToken);

                    records.Clear();

                    matchingMessagingService.MatchingPublish(new MatchingScoreCandidatesMessage());

                }, TBlockingQueue.BlockingFinished);

                matchingMessagingService.MatchingSubscribe<ClusteringPreProcessingStartedMessage>(async (message) =>
                {
                    var items = await clusteringRepository.GetAllAsync();

                    var records = items
                        .Cast<IDictionary<string, object>>()
                        .ToImmutableArray();

                    await ProcessAsync(records, stoppingToken);

                    records.Clear();

                    matchingMessagingService.MatchingPublish(new MatchingScoreOutstandingEdgesMessage());

                }, TMatchingQueue.ClusteringPreProcessingStarted);
            }, stoppingToken);
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred");
        }
    }

    private async Task ProcessAsync(IEnumerable<IDictionary<string, object>> records, CancellationToken stoppingToken)
    {
        statusMessagingService.StatusPublish(new StatusUpdateMessage("Comparing candidates..."));

        queue.Results.Clear();

        foreach(var record in records)
        {
            ComparisonResult result = new()
            {
                Record = record,
                MatchingKey = $"{record["l_SourceName"]}-{record["r_SourceName"]}"
            };

            var matching = matchingOptions[result.MatchingKey];

            foreach (var field in matching.ComparatorOptions)
            {
                object? source = record[field.Source];
                object? target = record[field.Target];

                MatcherResult[] matches = new MatcherResult[field.Comparators.Length];

                for (short comparator = 0; comparator < field.Comparators.Length; comparator++)
                {
                    IMatcher matcher = cache.Resolve(field.Comparators[comparator]);
                    matches[comparator] = matcher.Match(source, target);
                }

                result.Comparisons[field.Key] = matches;
            }

            queue.Results.Add(result);
        }

        await Task.CompletedTask;
    }

}
