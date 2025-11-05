namespace FileSync;

public class SyncOptions
{
    /// <summary>
    /// Indicates whether the application should process files and publish a message on startup.
    /// </summary>
    /// <remarks>
    /// When set to <c>true</c>, the application performs an initial file sync and publishes
    /// a message to notify the downstream system that files are ready for processing.
    /// </remarks>
    public required bool ProcessOnStartup { get; init; }

    /// <summary>
    /// Indicates whether the application should process files and publish a message
    /// after receiving confirmation that the previous batch completed successfully.
    /// </summary>
    /// <remarks>
    /// Triggered by <see cref="ClusteringPostProcessingFinishedMessage"/>.
    /// </remarks>
    public required bool ProcessOnCompletion { get; init; }

    /// <summary>
    /// Indicates whether the application should process files at a fixed time interval.
    /// </summary>
    /// <remarks>
    /// When enabled, the application will repeatedly process files based on the value
    /// of <see cref="ProcessTimerIntervalSeconds"/>.
    /// </remarks>
    public bool ProcessOnTimer { get; set; }

    /// <summary>
    /// The time interval, in seconds, between periodic processing runs.
    /// </summary>
    /// <remarks>
    /// This value is only used when <see cref="ProcessOnTimer"/> is set to <c>true</c>.
    /// It does not affect startup or completion-based processing, which are controlled
    /// separately by <see cref="ProcessOnStartup"/> and <see cref="ProcessOnCompletion"/>.
    /// </remarks>
    /// <value>
    /// An integer representing the number of seconds between each automatic processing cycle.
    /// A value less than or equal to zero disables interval-based processing.
    /// </value>
    public required int ProcessTimerIntervalSeconds { get; init; }
}