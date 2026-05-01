using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class DiamondGem : GemModel
{
    private bool _usedThisCombat;
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.ReplayStatic)];
    public override Color GemColor => new(0x97CADBFF);
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override CardRarity Rarity => CardRarity.Rare;

    private bool UsedThisCombat
    {
        get => _usedThisCombat;
        set
        {
            AssertMutable();
            _usedThisCombat = value;
        }
    }

    public override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }

    public override int ModifyPlayCount(int originalPlayCount)
    {
        return UsedThisCombat
            ? originalPlayCount
            : originalPlayCount +
              (int)GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
    }


    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (UsedThisCombat || cardPlay.Card != Card)
            return Task.CompletedTask;
        UsedThisCombat = true;
        return Task.CompletedTask;
    }
}