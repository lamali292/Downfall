using Downfall.DownfallCode.Abstract;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Maps a submod's card pool type to the <see cref="VotingPool"/> tab it appears
/// under in the art voting screen. Each submod registers its own pool from its
/// MainFile instead of this file listing every character by name, so a submod
/// can be excluded from the build without breaking this lookup.
/// </summary>
public static class VotingPoolRegistry
{
    private static readonly Dictionary<Type, VotingPool> Pools = new();

    public static void Register<TPool>(VotingPool pool) where TPool : IDownfallCardPool
    {
        Pools[typeof(TPool)] = pool;
    }

    public static bool TryGetPool(Type? cardPoolType, out VotingPool pool)
    {
        pool = default;
        return cardPoolType != null && Pools.TryGetValue(cardPoolType, out pool);
    }
}
