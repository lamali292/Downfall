using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class GrowthPower : SlimeBossPowerModel
{
    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Status) return;
        Flash();
        await PowerCmd.Decrement(this);
        await CardPileCmd.Draw(ctx, card.Owner);
    }
}