namespace Champ.ChampCode.Interfaces;

/// <summary>
/// A Finisher card's declared rules. The framework reads this one value for playability, glow, the description line
/// and execution (<see cref="Core.ChampCmd.PlayFinisher"/>), so text and behaviour cannot disagree.
/// A card that declares nothing gets <see cref="Default"/>: single-target, resolves once, clears the stance afterwards.
/// </summary>
/// <param name="AffectsAllPlayers">The stance's Finisher bonus applies to every player instead of only the owner.</param>
/// <param name="KeepsStance">
/// The stance is kept after the Finisher. By default it is cleared (an Ultimate stance is re-entered afterwards).
/// Declared as "keeps" so that <c>new()</c> / <c>default</c> (which bypass the parameter defaults) mean today's base behaviour.
/// </param>
/// <param name="RepeatCount">
/// How many times the Finisher resolves (stance bonus, OnFinisher hooks, history entry per repeat). Evaluated lazily at
/// play time because it may depend on X cost or a dynamic var; null means once.
/// </param>
public readonly record struct FinisherDescriptor(
    bool AffectsAllPlayers = false,
    bool KeepsStance = false,
    Func<int>? RepeatCount = null)
{
    public static FinisherDescriptor Default => new();

    public int Repeat => RepeatCount?.Invoke() ?? 1;
}
