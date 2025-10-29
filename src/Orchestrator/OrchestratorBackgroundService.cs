using FileStorage;
using Messaging.Messages.DbMessages.Receiving;
using Messaging.Messages.DbMessages.Sending;
using Messaging.Messages.MatchingMessages.Clustering;
using Messaging.Queues;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace Orchestrator;

/// <summary>
/// This is an 'ad-hoc' service intended to aid the process of bulk loading files into DMS.
/// </summary>
public class OrchestratorBackgroundService(
    ILogger<OrchestratorBackgroundService> logger,
    IConfiguration configuration,
    IMatchingMessagingService matchingMessagingService,
    IDbMessagingService dbMessagingService,
    IFileLocations fileLocations) : BackgroundService
{ 
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var files = configuration.GetValue<string>("FilesLocation") ?? throw new Exception("Missing configuration section for FilesLocation");
            var scriptLocation = configuration.GetValue<string>("ScriptLocation") ?? throw new Exception("Missing configuration section for ScriptLocation");

            var offlocFilePattern = "*.dat";
            var deliusFilePattern = "*.txt";

            if (TryReadScript(scriptLocation, out string? script) is false)
            {
                throw new Exception("Could not find script!");
            }

            // Create and open a Runspace
            var runspace = RunspaceFactory.CreateRunspace();
            runspace.Open();

            // Create a single PowerShell instance
            var powershell = PowerShell.Create();
            powershell.Runspace = runspace;
            powershell.AddScript(script, true);

            await Task.Run(() =>
            {
                matchingMessagingService.MatchingSubscribe<ClusteringPostProcessingFinishedMessage>(async message =>
                {
                    logger.LogInformation($"{nameof(ClusteringPostProcessingFinishedMessage)} received.");

                    var allOfflocFiles = GetAllFilesByPattern(files, offlocFilePattern);
                    var allDeliusFiles = GetAllFilesByPattern(files, deliusFilePattern);

                    if (allOfflocFiles is { Length: 0 } || allDeliusFiles is { Length: 0 })
                    {
                        logger.LogInformation($"Missing files in '{files}'. Skipping further execution...");
                        return;
                    }

                    var processedOfflocFiles = await GetAlreadyProcessedOfflocFilesAsync();
                    var processedDeliusFiles = await GetAlreadyProcessedDeliusFilesAsync();

                    var unprocessedOfflocFiles = GetUnprocessedFiles(allOfflocFiles, processedOfflocFiles);
                    var unprocessedDeliusFiles = GetUnprocessedFiles(allDeliusFiles, processedDeliusFiles);

                    logger.LogInformation("Finding unprocessed files...");

                    var offlocFileToProcess = unprocessedOfflocFiles.OrderBy(fileName =>
                    {
                        var y = fileName.Key.Substring(17, 8);
                        return DateOnly.Parse($"{y[..2]}/{y[2..4]}/{y[4..]}");
                    }).FirstOrDefault();

                    var deliusFileToProcess = unprocessedDeliusFiles.OrderBy(fileName =>
                    {
                        return DateOnly.Parse($"{fileName.Key[27..29]}/{fileName.Key[25..27]}/{fileName.Key[21..25]}");
                    }).FirstOrDefault();

                    logger.LogInformation("Find unprocessed files done.");

                    if (offlocFileToProcess is { Key: null } || deliusFileToProcess is { Key: null })
                    {
                        logger.LogInformation("No new file(s) to process. Skipping further execution...");
                        return;
                    }

                    logger.LogInformation($"Copying file '{offlocFileToProcess.Key}' to '{fileLocations.offlocInput}'...");
                    File.Copy(offlocFileToProcess.Value, Path.Combine(fileLocations.offlocInput, offlocFileToProcess.Key), true);
                    logger.LogInformation("Copy file done.");

                    logger.LogInformation($"Copying file '{deliusFileToProcess.Key}' to '{fileLocations.deliusInput}'...");
                    File.Copy(deliusFileToProcess.Value, Path.Combine(fileLocations.deliusInput, deliusFileToProcess.Key), true);
                    logger.LogInformation("Copy file done.");

                    await ExecuteScriptAsync(powershell, stoppingToken);

                }, TMatchingQueue.ClusteringPostProcessingFinished);

            }, stoppingToken);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred");
        }

    }

    private static bool TryReadScript(string location, out string script)
    {
        script = string.Empty;

        if(File.Exists(location) is false)
        {
            return false;
        }

        script = File.ReadAllText(location);

        return true;
    }

    private async Task ExecuteScriptAsync(PowerShell powershell, CancellationToken stoppingToken)
    {
        logger.LogInformation("Executing (powershell) script...");

        try
        {
            await powershell.InvokeAsync();

            foreach(var error in powershell.Streams.Error)
            {
                logger.LogError($"Powershell: {error}");
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
       
        logger.LogInformation("Execution complete.");
    }

    private static IEnumerable<KeyValuePair<string, string>> GetUnprocessedFiles(string[] allFiles, string[] processedFiles)
    {
        return allFiles.ToDictionary(file => Path.GetFileName(file), filePath => filePath)
            .Where(file => !processedFiles.Contains(file.Key));
    }

    private string[] GetAllFilesByPattern(string location, string pattern)
    {
        logger.LogInformation($"Retrieving '{pattern}' files from '{location}'...");
        var files = Directory.GetFiles(location, pattern, SearchOption.AllDirectories);
        logger.LogInformation("Retrieving files done.");

        return files;
    }

    private async Task<string[]> GetAlreadyProcessedOfflocFilesAsync()
    {
        logger.LogInformation("Getting processed Offloc files...");
        var offloc = await dbMessagingService.DbTransientSubscribe<GetOfflocFilesMessage, OfflocFilesReturnMessage>(new GetOfflocFilesMessage());
        logger.LogInformation("Get processed files done.");

        return offloc.offlocFiles;
    }

    private async Task<string[]> GetAlreadyProcessedDeliusFilesAsync()
    {
        logger.LogInformation("Getting processed Delius files...");
        var delius = await dbMessagingService.DbTransientSubscribe<GetDeliusFilesMessage, DeliusFilesReturnMessage>(new GetDeliusFilesMessage());
        logger.LogInformation("Get processed files done.");

        return delius.fileNames;
    }

}
