using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.ValueProps;
namespace Collector.CollectorCode.Cards.Collectibles;

public class TheInsatiableCard : Collectible<TheInsatiableBoss>
{
    public TheInsatiableCard() : base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy, 0.61f)
    {
        WithCalculatedDamage(25, 5, StatusAndCurseCount, DamageProps.card, 5, 1);
        WithTip(CardKeyword.Exhaust);
    }

    private static decimal StatusAndCurseCount(CardModel card, Creature? creature) =>
        card.Owner.ExhaustPile.Count(c => c.Type is CardType.Status or CardType.Curse);

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}