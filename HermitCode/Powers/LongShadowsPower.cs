using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

public class LongShadowsPower : HermitPowerModel
{
    public override Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var player = card.Owner;
        if (player.Creature != Owner || card.Type != CardType.Curse) return Task.CompletedTask;
        return CardPileCmd.Draw(ctx, Amount, player);
    }
}