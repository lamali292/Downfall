using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class DefensiveFlair : SneckoCardModel, IHasGift
{
    public DefensiveFlair() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        this.WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithCalculatedBlock(8, 2, CalcBlock, ValueProp.Move, 1, 1);
    }

    public Gift? Gift { get; set; }

    private static decimal CalcBlock(CardModel card, Creature? creature)
    {
        return card.Owner.GetHand().Count(SneckoCmd.IsOffclass);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}