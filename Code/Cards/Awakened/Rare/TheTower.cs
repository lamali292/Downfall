using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class TheTower : AwakenedCardModel
{
    public TheTower() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithCalculatedDamage(2, 2, DamageCalc, ValueProp.Move, 1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target, DynamicVars.CalculatedDamage).Execute(ctx);
    }

    private static decimal DamageCalc(CardModel card, Creature? creature)
    {
        return CombatManager.Instance.History.Entries
            .OfType<CardGeneratedEntry>()
            .Count(e => e.GeneratedByPlayer && e.Card.Owner == card.Owner);
    }
}