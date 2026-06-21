using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class LuckyBreak : SneckoCardModel
{
    public LuckyBreak() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 3);
        WithCards(1);
    }


    private int TwoCostInHand => Owner.GetHand()
        .Count(e => e.EnergyCost.GetResolved() >= 2);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CardPileCmd.Draw(ctx, TwoCostInHand, Owner);
    }
}