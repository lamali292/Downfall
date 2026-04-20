using Downfall.Code.Commands;
using Downfall.Code.Core.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Gems;

public class AmberGem : GemModel
{
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Accelerate(cardPlay.Card.Owner);
    }
}