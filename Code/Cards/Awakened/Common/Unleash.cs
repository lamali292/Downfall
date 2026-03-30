using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Unleash : AwakenedCardModel
{
    public Unleash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithVars(
            new CalculationBaseVar(5m),
            new ExtraDamageVar(1m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(DamageCalc)
        );
        //WithCalculatedDamage(12, 1, DamageCalc);
    }

    private static decimal DamageCalc(CardModel card, Creature? creature)
    {
        return PileType.Hand.GetPile(card.Owner).Cards.Count(c => c != card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target, DynamicVars.CalculatedDamage).Execute(ctx);
    }
}