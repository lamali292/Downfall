using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Keywords;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Gems;

public class SapphireGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [DownfallTip.Brace.ToHoverTip()];

    public override Color GemColor => new(0x0624BEFF);
    public override CardRarity Rarity => CardRarity.Common;
    
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(ctx, cardPlay.Card.Owner, 4);
    }
}