using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Downfall.Code.Gems;

public class SapphireGem : GemModel
{
    public override IEnumerable<IHoverTip> ExtraHoverTips => [DownfallTip.Brace.ToHoverTip()];

    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(cardPlay.Card.Owner, 4);
    }
}