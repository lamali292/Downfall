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

public class SapphireGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [GuardianTip.Brace.ToHoverTip()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(4)];
    public override Color GemColor => new(0x0624BEFF);
    public override CardRarity Rarity => CardRarity.Common;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await GuardianCmd.Brace(ctx, Player, effect);
    }
}