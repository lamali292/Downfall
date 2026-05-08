using Downfall.DownfallCode.Powers;

using Godot;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class EmeraldGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TemporaryDexterityUpPower>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(2)];
    public override Color GemColor => new(0x319028FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var owner = Player.Creature;
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await PowerCmd.Apply<TemporaryDexterityUpPower>(ctx, owner, effect, owner, null);
    }
}