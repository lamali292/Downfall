using Downfall.DownfallCode.Commands;
using Godot;
using Guardian.GuardianCode.Cards.Token;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Guardian.GuardianCode.Gems;

public class AquamarineGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<CrystalWard>()];

    public override Color GemColor => new(0x06A5BEFF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<CrystalWard>(cardPlay.Card.Owner, PileType.Hand);
    }
}