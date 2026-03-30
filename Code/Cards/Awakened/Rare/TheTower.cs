using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class TheTower : AwakenedCardModel
{
    public TheTower() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithVars(
            new CalculationBaseVar(2m),
            new ExtraDamageVar(2m),
            new CalculatedDamageVar(ValueProp.Move).WithMultiplier(DamageCalc)
        );
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay.Target, DynamicVars.CalculatedDamage).Execute(ctx);
    }

    private static decimal DamageCalc(CardModel card, Creature? creature) => CombatManager.Instance.History.Entries
        .OfType<CardGeneratedEntry>()
        .Count(e => e.GeneratedByPlayer && e.Card.Owner == card.Owner);

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(1);
        DynamicVars.ExtraDamage.UpgradeValueBy(1);
    }
}