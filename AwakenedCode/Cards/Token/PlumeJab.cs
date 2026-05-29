using Awakened.AwakenedCode.Powers;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Awakened.AwakenedCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class PlumeJab : AwakenedCardModel
{
    public PlumeJab() : base(0, CardType.Attack, CardRarity.Token, TargetType.RandomEnemy)
    {
        WithDamage(2, 1);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
        this.WithRepeat(2);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars.Repeat.IntValue).Execute(ctx);
    }

    // Razor Sharp stuff. 
    public override Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (card != this) return Task.CompletedTask;
        var a = Owner.Creature.GetPowerAmount<RazorSharpPower>();
        if (a == 0) return Task.CompletedTask;
        DynamicVars.Repeat.UpgradeValueBy(a);
        return Task.CompletedTask;
    }


    public override Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is RazorSharpPower && power.Owner == Owner.Creature) DynamicVars.Repeat.UpgradeValueBy(amount);
        return Task.CompletedTask;
    }
}