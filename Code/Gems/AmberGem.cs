using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Keywords;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Gems;

public class AmberGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [DownfallTip.Accelerate.ToHoverTip()];

    public override Color GemColor => new(0xD0D100FF);
    public override CardRarity Rarity => CardRarity.Uncommon;

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Accelerate(ctx, cardPlay.Card.Owner);
    }
}