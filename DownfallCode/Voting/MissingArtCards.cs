using Automaton.AutomatonCode.Core;
using Awakened.AwakenedCode.Core;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Abstract;
using Guardian.GuardianCode.Core;
using Hermit.HermitCode.Core;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using Snecko.SneckoCode.Core;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Domain logic for "which cards are open for art submission" - shared by
/// the voting grid, the upload popup and the card picker, none of which
/// should need to know about each other to answer this question themselves.
/// </summary>
public static class MissingArtCards
{
    // Cards render at 500x380 (or an integer 2x hi-res variant); Ancients use
    // a taller portrait frame. Submitted art must match exactly so it drops
    // into the card frame without being rescaled/cropped unpredictably.
    private static readonly (int w, int h)[] NormalSizes = [(500, 380), (1000, 760)];
    private const int AncientWidth = 606;
    private const int AncientHeight = 852;

    /// <summary>
    /// Every Downfall card whose portrait still resolves to its "todo.tres"
    /// placeholder (see StringExtensions.CardImageAtlasPath). Derived from
    /// game data, not a server-curated list.
    /// </summary>
    public static List<ArtData> ComputeAll()
    {
        return ModelDb.AllCards
            .Where(card => TryGetPool(card, out _) &&
                           card is DownfallCardModel { CustomPortraitPath: { } path } &&
                           path.EndsWith("todo.tres"))
            .Select(card => new ArtData { ModelId = card.Id })
            .ToList();
    }

    public static VotingPool PoolFor(CardModel? card)
    {
        TryGetPool(card, out var pool);
        return pool;
    }

    public static bool TryGetPool(CardModel? card, out VotingPool pool)
    {
        switch (card?.Pool)
        {
            case AutomatonCardPool: pool = VotingPool.Automaton; return true;
            case AwakenedCardPool:  pool = VotingPool.Awakened;  return true;
            case ChampCardPool:     pool = VotingPool.Champ;     return true;
            case GuardianCardPool:  pool = VotingPool.Guardian;  return true;
            case HermitCardPool:    pool = VotingPool.Hermit;    return true;
            case HexaghostCardPool: pool = VotingPool.Hexaghost; return true;
            case SlimeBossCardPool: pool = VotingPool.Slimeboss; return true;
            case SneckoCardPool:    pool = VotingPool.Snecko;    return true;
            default:
                pool = default;
                return false;
        }
    }

    public static bool IsAncient(ArtData? category) => category?.Card?.Rarity == CardRarity.Ancient;

    public static string SizeRequirementText(ArtData? category) => IsAncient(category)
        ? VotingUi.Loc("DOWNFALL-VOTING.size_hint_ancient")
        : VotingUi.Loc("DOWNFALL-VOTING.size_hint_normal");

    public static bool IsValidSize(int width, int height, ArtData? category) => IsAncient(category)
        ? width == AncientWidth && height == AncientHeight
        : NormalSizes.Any(s => s.w == width && s.h == height);
}
