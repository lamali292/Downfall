using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Ancient;

public class Crackshot : HermitCardModel, IHasDeadOnEffect
{
    public Crackshot() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
    }

    public override bool GainsBlock => true;
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        var result = await CommonActions.CardAttack(this, cardPlay)
            .WithHermitGunHitFx().BeforeDamage(() =>
            {
                HermitSfx.PlayGun1();
                return Task.CompletedTask;
            })
            .Execute(ctx);
        var unblockedDamage = result.Results.SelectMany(e => e).Sum(e => e.TotalDamage);
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, ValueProp.Move, cardPlay);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (this is not IHasDeadOnEffect deadOnEffect) return 1;
        if (cardSource != this || dealer != Owner.Creature || !props.IsPoweredAttack() || !deadOnEffect.IsDeadOn)
            return 1;
        return Owner.Creature.HasPower<SnipePower>() ? 4 : 2;
    }

    
    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }
}