using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class AshesToAshesPower : CollectorPowerModel
{
    public AshesToAshesPower()
    {
        WithTip<Ember>();
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner || card is not Ember)
            return;
        Flash();
        for (var i = 0; i < Amount; i++) CardCmd.Upgrade(card);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner.Creature != Owner || card.Type is not (CardType.Curse or CardType.Status)) return;
        await CardCmd.TransformTo<Ember>(card);
    }

}