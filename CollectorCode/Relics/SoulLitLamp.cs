using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class SoulLitLamp : CollectorRelicModel
{
    public SoulLitLamp() : base(RelicRarity.Uncommon)
    {
        WithCardTip<Ember>();
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext ctx, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner || card.Type is not (CardType.Status or CardType.Curse) || card is Ember) return;
        Flash();
        await CardCmd.TransformTo<Ember>(card);
    }
}