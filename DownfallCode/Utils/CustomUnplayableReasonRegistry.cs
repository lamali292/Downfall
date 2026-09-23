using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Utils;

/// <summary>
/// Lets a submod attach a specific "why can't I play this" thought-bubble line to a card blocked by its own
/// <c>IsPlayable</c> override (UnplayableReason.BlockedByCardLogic), instead of the generic
/// "combat_messages.UNPLAYABLE" text every such card shows by default.
/// Give <see cref="Flag"/> its value via a [CustomEnum] static UnplayableReason field so it doesn't collide
/// with base-game or other mods' flags. See <see cref="CustomUnplayableReasonRegistry"/> to register one.
/// </summary>
public interface ICustomUnplayableReason
{
    /// <summary>Flag bit identifying this reason. Assign with a [CustomEnum] static UnplayableReason field.</summary>
    UnplayableReason Flag { get; }

    /// <summary>Whether this is specifically why <paramref name="card"/> is currently unplayable.</summary>
    bool AppliesTo(CardModel card);

    /// <summary>The thought-bubble line to show instead of the generic "can't play this" text.</summary>
    LocString GetDialogueLine(CardModel card);
}

public static class CustomUnplayableReasonRegistry
{
    private static readonly List<ICustomUnplayableReason> _reasons = [];

    public static void Register(ICustomUnplayableReason reason) => _reasons.Add(reason);

    public static IReadOnlyList<ICustomUnplayableReason> All => _reasons;
}
