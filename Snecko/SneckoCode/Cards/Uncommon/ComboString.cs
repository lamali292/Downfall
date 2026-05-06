using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class ComboString : SneckoCardModel
{
    public ComboString() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithGift(new Gift
        {
            Rarity = CardRarity.Uncommon
        });
        WithCalculatedDamage(0, 7, CalcDamage, ValueProp.Move, 0, 2);
    }

    private static decimal CalcDamage(CardModel card, Creature? creature)
    {
        return CombatManager.Instance.History
            .CardPlaysFinished.Count(e =>
                e.HappenedThisTurn(card.CombatState) &&
                SneckoCmd.IsOffclass(card, e.CardPlay.Card));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}