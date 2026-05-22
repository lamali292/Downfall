using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class DefensiveFlair : SneckoCardModel
{
    public DefensiveFlair() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithCalculatedBlock(8, 2, CalcBlock, ValueProp.Move, 1, 1);
    }

    private static decimal CalcBlock(CardModel card, Creature? creature)
    {
        return card.Owner.GetHand().Count(e => SneckoCmd.IsOffclass(card, e));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, DynamicVars.CalculatedBlock, cardPlay);
    }
}