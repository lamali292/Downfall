using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public class LongShadowsPower : HermitPowerModel
{
    public override Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        var player = card.Owner;
        if (player.Creature != Owner || card.Type != CardType.Curse) return Task.CompletedTask;
        Flash();
        return CardPileCmd.Draw(ctx, Amount, player);
    }
}