using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Awakened;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class PlumeJab : AwakenedCardModel
{
    public PlumeJab() : base(0, CardType.Attack, CardRarity.Token, TargetType.RandomEnemy)
    {
        WithDamage(2);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        WithVars(new RepeatVar(2));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
    }


    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1);
    }


    // Razor Sharp stuff. 
    public override Task AfterCardGeneratedForCombat(CardModel card, bool addedByPlayer)
    {
        if (card != this) return Task.CompletedTask;
        var a = Owner.Creature.GetPowerAmount<RazorSharpPower>();
        if (a == 0) return Task.CompletedTask;
        DynamicVars.Repeat.UpgradeValueBy(a);
        return Task.CompletedTask;
    }


    public override Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power is RazorSharpPower && power.Owner == Owner.Creature) DynamicVars.Repeat.UpgradeValueBy(amount);
        return Task.CompletedTask;
    }
}