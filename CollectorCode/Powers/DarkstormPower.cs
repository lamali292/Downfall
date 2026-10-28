using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

/// <summary>Darkstorm: drawing an Embered Kindling draws another card.</summary>
public class DarkstormPower : CollectorPowerModel
{
    public DarkstormPower()
    {
        WithTip<Ember>();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card is not Ember || card.Owner.Creature != Owner) return;
        Flash();
        await CardPileCmd.Draw(ctx, Amount, card.Owner);
    }
}
