using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class SoulStrike : AwakenedCardModel
{
    public SoulStrike() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(4);
        WithVars(new RepeatVar(3));
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitCount(DynamicVars.Repeat.IntValue)
            .Execute(ctx);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2);
    }

    public override Task AfterCardEnteredCombat(CardModel card)
    {
        if (card != this || IsClone)
            return Task.CompletedTask;
        ReduceCostBy(CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            e.CardPlay.Card.Type == CardType.Power
            && e.CardPlay.Card.Owner == Owner
            && e.HappenedThisTurn(CombatState)
        ));
        return Task.CompletedTask;
    }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Power)
            return Task.CompletedTask;
        ReduceCostBy(1);
        return Task.CompletedTask;
    }

    private void ReduceCostBy(int amount)
    {
        EnergyCost.AddThisTurn(-amount);
    }
}