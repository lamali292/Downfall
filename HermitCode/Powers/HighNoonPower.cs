using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

public sealed class HighNoonPower : HermitPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;
        if (!cardPlay.Card.Tags.Contains(CardTag.Strike) &&
            !cardPlay.Card.Tags.Contains(CardTag.Defend)) return;
        Flash();
        await CardPileCmd.Draw(context, Amount, Owner.Player!);
    }
}