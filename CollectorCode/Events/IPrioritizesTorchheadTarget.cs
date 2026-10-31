namespace Collector.CollectorCode.Events;

/// <summary>
///     Marks a power whose owner Torchhead attacks before any other enemy (the lowest-HP one if several have it),
///     instead of the enemy its owner hit last.
/// </summary>
public interface IPrioritizesTorchheadTarget;
