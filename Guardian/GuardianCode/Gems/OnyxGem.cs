using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class OnyxGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(GuardianTip.Polish)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override Color GemColor => new(0x616161FF);
    public override CardRarity Rarity => CardRarity.Rare;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await GuardianCmd.Polish(ctx, Card, effect);
    }
}