using Godot;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Gems;

public class DiamondGem : GemModel
{
    public override Color GemColor => new(0x97CADBFF);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override CardRarity Rarity => CardRarity.Rare;

    public override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.ReplayStatic),
        HoverTipFactory.Static(StaticHoverTip.Energy),
        HoverTipFactory.Static(GuardianTip.Aggravate)
    ];

    private bool UsedThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    // Diamond has no OnPlayInternal effect of its own - its one effect (extra replays of the host
    // card) is decided once, up front, inside ModifyPlayCount. OnPlay then still fires again for
    // every physical replay that single decision caused; only the first (PlayIndex 0) should be
    // reported as "an activation" - the rest are consequences of that one decision, not fresh ones,
    // and reporting them too would let Temporal Refraction's single doubling drain extra stacks.
    protected override bool ReportsActivation(CardPlay cardPlay) => cardPlay.PlayIndex == 0;

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay,
        IEnumerable<Player> targetPlayers)
    {
        return Task.CompletedTask;
    }

    public override int ModifyPlayCount(int originalPlayCount)
    {
        if (UsedThisCombat || Card == null) return originalPlayCount;

        // don't run on canonical instance
        if (!IsMutable || !Card.IsMutable) return originalPlayCount;

        var owner = Card?.Owner;
        if (owner == null) return originalPlayCount;
        var combatState = owner.Creature.CombatState;
        if (combatState == null) return originalPlayCount;
        return originalPlayCount +
               (int)GuardianHook.ModifyGemEffect(combatState, this, DynamicVars.Gem.BaseValue, Card);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (UsedThisCombat || cardPlay.Card != Card)
            return Task.CompletedTask;
        UsedThisCombat = true;
        return Task.CompletedTask;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (Card is IGemCard || card != Card) return false;
        modifiedCost++;
        return true;
    }
}