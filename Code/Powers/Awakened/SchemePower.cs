using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Token;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Powers.Awakened;

public class SchemePower : AwakenedPowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.IsAutoPlay || cardPlay.Card is Scheme) return;
        var dupe = cardPlay.Card.CreateDupe();
        await CardCmd.AutoPlay(ctx, dupe, cardPlay.Target);
        await PowerCmd.Decrement(this);
    }
}