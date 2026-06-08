using Downfall.DownfallCode.Commands;
using Godot;
using Guardian.GuardianCode.Cards.Token;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Guardian.GuardianCode.Gems;

public class AquamarineGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<CrystalWard>()];

    public override Color GemColor => new(0x06A5BEFF);
    public override CardRarity Rarity => CardRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new GemVar(1)];

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay? cardPlay)
    {
        var effect = GuardianHook.ModifyGemEffect(CombatState, this, DynamicVars.Gem().BaseValue, Card);
        await DownfallCardCmd.GiveCards<CrystalWard>(Player, PileType.Hand, effect);
    }
}