using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class FlashInThePan : SneckoCardModel
{
    public FlashInThePan() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(13, 3);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var cards = Owner.GetHand();
        var amount = cards.Count;
        if (amount == 0) return;
        await CardCmd.Discard(ctx, cards);
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, Owner.Creature, amount, Owner.Creature, this);
    }
}