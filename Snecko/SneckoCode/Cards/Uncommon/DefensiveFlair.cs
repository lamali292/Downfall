using BaseLib.Utils;
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
        return card.Owner.PlayerCombatState?.Hand.Cards.Count(e => SneckoCmd.IsOffclass(card, e)) ?? 0;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, DynamicVars.CalculatedBlock, cardPlay);
    }
}