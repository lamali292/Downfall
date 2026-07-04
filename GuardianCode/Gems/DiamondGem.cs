using Godot;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Gems;

public class DiamondGem : GemModel
{
    private bool _usedThisCombat;
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
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
        }
    }

    protected override Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay)
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
               (int)GuardianHook.ModifyGemEffect(combatState, this, DynamicVars.Gem().BaseValue, Card);
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
        if (Card is  IGemCard || card != Card) return false;
        modifiedCost++;;
        return true;
    }
}