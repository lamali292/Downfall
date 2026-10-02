using Downfall.DownfallCode.Abstract;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Maps a submod's card pool type to the <see cref="VotingPool"/> tab it appears
/// under in the art voting screen, and the character icon shown on that tab's
/// toggle button. Each submod registers itself from its MainFile instead of
/// this file (or the voting UI) listing every character by name, so a submod
/// can be excluded from the build without breaking this lookup, and adding a
/// new character only takes one line here - no scene edits.
/// </summary>
public static class VotingPoolRegistry
{
    private static readonly Dictionary<Type, VotingPool> Pools = new();
    private static readonly Dictionary<VotingPool, string> IconPaths = new();
    private static readonly List<VotingPool> RegistrationOrder = new();

    /// <param name="modId">The submod's ModId (e.g. "Automaton") - used to resolve
    /// the character icon at <c>res://{modId}/images/character/character_icon.png</c>,
    /// same convention as <see cref="DownfallCharacterModel.CustomIconTexturePath"/>.</param>
    public static void Register<TPool>(VotingPool pool, string modId) where TPool : IDownfallCardPool
    {
        Pools[typeof(TPool)] = pool;

        if (IconPaths.TryAdd(pool, $"res://{modId}/images/character/character_icon.png"))
            RegistrationOrder.Add(pool);
    }

    public static bool TryGetPool(Type? cardPoolType, out VotingPool pool)
    {
        pool = default;
        return cardPoolType != null && Pools.TryGetValue(cardPoolType, out pool);
    }

    /// <summary>Every pool that registered itself, in registration order - drives the
    /// dynamically-built pool filter buttons in <see cref="NVotingFilter"/>.</summary>
    public static IReadOnlyList<VotingPool> RegisteredPools => RegistrationOrder;

    public static string? IconPath(VotingPool pool) => IconPaths.GetValueOrDefault(pool);
}
