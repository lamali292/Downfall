using BaseLib.Utils;
using Hermit.HermitCode.Powers;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Common;

public class Headshot : HermitCardModel, IHasDeadOnEffect
{
    public Headshot() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
    }


    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource != this || dealer != Owner.Creature || !props.IsPoweredAttack() || !IsDeadOn)
            return 1;
        return Owner.Creature.HasPower<SnipePower>() ? 4 : 2;
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();

        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);
    }

    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
     => Task.CompletedTask;
}