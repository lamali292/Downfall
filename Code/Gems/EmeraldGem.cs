using Downfall.Code.Core.Guardian;
using Downfall.Code.Powers.Downfall;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Gems;

public class EmeraldGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<TemporaryDexterityUpPower>()];

    public override Color GemColor => new(0x319028FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner.Creature;
        await PowerCmd.Apply<TemporaryDexterityUpPower>(ctx, owner, 2, owner, null);
    }
}