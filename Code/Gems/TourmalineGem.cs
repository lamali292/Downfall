using Downfall.Code.Core.Guardian;
using Downfall.Code.Powers.Downfall;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Gems;

public class TourmalineGem : GemModel
{
    public override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        await PowerCmd.Apply<TemporaryThornsPower>(owner.Creature, 4, owner.Creature, null);
    }
}