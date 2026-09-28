using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Compatibility;

public static class GameVersion
{
    /// <summary>New card-play API: CardLocation struct, Hook.ModifyCardPlayResultLocation, CardPlay.Player.</summary>
    public static readonly bool HasCardLocation =
        AccessTools.TypeByName("MegaCrit.Sts2.Core.Entities.Cards.CardLocation") != null;

    /// <summary>NCard portrait API rename.</summary>
    public static readonly bool HasNCardUpdatePortrait =
        AccessTools.Method(typeof(NCard), "UpdatePortrait") != null;

    // On beta, CombatManager.ResolveTurnEndCardEffects runs card.OnTurnEndInHandWrapper() and then
    // separately moves the card to Discard. On main, that final Discard move happens inline at the
    // end of CardModel.OnTurnEndInHandWrapper itself - ResolveTurnEndCardEffects doesn't exist there.
    /// <summary>Beta-branch-only: CombatManager.ResolveTurnEndCardEffects exists.</summary>
    public static readonly bool HasResolveTurnEndCardEffects =
        AccessTools.Method(typeof(CombatManager), "ResolveTurnEndCardEffects") != null;

    // CardPileAddResult.targetPile was removed on some builds; cardAdded.Pile?.Type covers the same
    // info except for the "tried to add to a full hand, got sent to discard" edge case.
    private static readonly FieldInfo? CardPileAddResultTargetPileField =
        AccessTools.Field(typeof(CardPileAddResult), "targetPile");

    /// <summary>Version-safe replacement for CardPileAddResult.targetPile.</summary>
    public static PileType? TargetPileCompat(this CardPileAddResult result) =>
        CardPileAddResultTargetPileField?.GetValue(result) as PileType?;

    // AttackCommand.CardPlay (which CardPlay triggered this attack) was added alongside the
    // CardLocation card-play rework; older builds have no such property at all.
    private static readonly PropertyInfo? AttackCommandCardPlayProp =
        AccessTools.Property(typeof(AttackCommand), "CardPlay");

    /// <summary>Version-safe replacement for `AttackCommand.CardPlay = cardPlay` (no-op if the property doesn't exist).</summary>
    public static void SetCardPlayCompat(this AttackCommand command, CardPlay? cardPlay) =>
        AttackCommandCardPlayProp?.SetValue(command, cardPlay);

    // CardCreationOptions.WithCardPools gained a second `cardPoolFilter` parameter on some builds
    // (e.g. current live/main game branch) while older builds (e.g. beta) still only have the
    // single-arg overload. The exact overload is baked into the call site at compile time, so a
    // build compiled against one branch throws MissingMethodException on the other - resolve the
    // overload at runtime instead.
    private static readonly MethodInfo? WithCardPoolsMethod =
        AccessTools.Method(typeof(CardCreationOptions), nameof(CardCreationOptions.WithCardPools));

    /// <summary>Version-safe replacement for `CardCreationOptions.WithCardPools(pools)`.</summary>
    public static CardCreationOptions WithCardPoolsCompat(this CardCreationOptions options, IEnumerable<CardPoolModel> pools)
    {
        if (WithCardPoolsMethod == null) return options;
        var args = WithCardPoolsMethod.GetParameters().Length == 1
            ? new object?[] { pools }
            : new object?[] { pools, options.CardPoolFilter };
        return (CardCreationOptions)WithCardPoolsMethod.Invoke(options, args)!;
    }
}