using Downfall.Code.Cards.Guardian.Token;
using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Gems;

public class FragmentedGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<CrystalShiv>()];

    public override Color GemColor => new(0xCE1AB2FF);
    public override CardRarity Rarity => CardRarity.Common;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await DownfallCardCmd.GiveCard<CrystalShiv>(cardPlay.Card.Owner, PileType.Hand);
    }
}