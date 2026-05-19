using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class CitrineGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Energy)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new EnergyVar(1)];
    public override Color GemColor => new(0xA55B08FF);
    public override CardRarity Rarity => CardRarity.Rare;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Energy.BaseValue, Card);
        await PlayerCmd.GainEnergy(effect, Player);
    }
}