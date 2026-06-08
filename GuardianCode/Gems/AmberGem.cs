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

public class AmberGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(GuardianTip.Accelerate)];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];
    public override Color GemColor => new(0xD0D100FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await GuardianCmd.Accelerate(ctx, Player, (int)effect);
    }
}