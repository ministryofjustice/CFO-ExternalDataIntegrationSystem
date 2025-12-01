using Messaging.Messages.MatchingMessages;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Messages.StatusMessages;
using Messaging.Queues;

namespace Matching.Engine.Services;

public class ScorerService(
    IMessageService matchingMessagingService,
    IMatchingRepository matchingRepository,
    IClusteringRepository clusteringRepository,
    ILogger<ScorerService> logger,
    IOptions<List<MatchingOption>> options,
    MatchingQueue queue,
    BayesCache bayesCache) : BackgroundService
{
    Dictionary<string, MatchingOption> matchingOptions;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            matchingOptions = options.Value.ToDictionary(o => o.MatchingKey);

            await matchingMessagingService.SubscribeAsync<MatchingScoreCandidatesMessage>(async (message) =>
            {
                var results = await ScoreAsync(stoppingToken);
                await matchingRepository.BulkInsertAsync(results);
                results = null;

                await matchingMessagingService.PublishAsync(new MatchingScoreCandidatesFinishedMessage());
            }, TMatchingQueue.MatchingScoreCandidatesMessage);

            await matchingMessagingService.SubscribeAsync<MatchingScoreOutstandingEdgesMessage>(async (message) =>
            {
                var results = await ScoreAsync(stoppingToken);
                await clusteringRepository.BulkInsertAsync(results);
                results = null;

                await matchingMessagingService.PublishAsync(new ClusteringPreProcessingFinishedMessage());
            }, TMatchingQueue.MatchingScoreOutstandingEdgesMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred");
        }

    }

    private async Task<Dictionary<ComparisonResult, double>> ScoreAsync(CancellationToken stoppingToken)
    {
        Dictionary<ComparisonResult, double> results = [];

        foreach (var item in queue.Results)
        {
            var matching = matchingOptions[item.MatchingKey];

            var scoreOptions = matching.ScoringOptions;
            double prior = scoreOptions.Prior;

            var fields = scoreOptions.Fields.ToDictionary(f => f.FieldKey);

            var result = item;

            double[] bayes = new double[result.Comparisons.Count];

            for (int i = 0; i < result.Comparisons.Count; i++)
            {
                var comparison = result.Comparisons.ElementAt(i);

                string fieldKey = comparison.Key;
                var field = fields[fieldKey];

                IEnumerable<MatcherResult> matches = comparison.Value;

                List<double> calculatedBayes = new(result.Comparisons.Count);

                for (int m = 0; m < matches.Count(); m++)
                {
                    var comparatorKey = field.BayesFactors[m].ComparatorKey;

                    var match = matches.ElementAt(m);
                    var score = CalculateBayes($"{item.MatchingKey}.{fieldKey}.{comparatorKey}", match);
                    calculatedBayes.Add(score);
                }

                bayes[i] = calculatedBayes.Max();
            }

            double probability = GetProbability(prior, bayes);

            results.Add(result, probability);
        }

        queue.Results.Clear();

        return await Task.FromResult(results);
    }

    public double CalculateBayes(string key, MatcherResult match)
    {
        var bayes = bayesCache.Resolve(key);
        return bayes.GetScore(match);
    }

    private static double GetProbability(double prior, params double[] bayes)
    {
        double sum = prior + bayes.Sum();
        double exponent = Math.Pow(2, sum);
        double probability = Math.Min(exponent / (1 + exponent), 1);
        return probability;
    }
}
