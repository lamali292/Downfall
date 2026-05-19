using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 10 damage. Double this card's damage and increase its cost by 1.
///     Upgrade: 12 damage.
/// </summary>
public sealed class Desperado : HermitCardModel
{
    public Desperado() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(10, 2);
        WithVar("PlayCountMultiplier", 1);
    }


    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (cardSource != this || dealer != Owner.Creature || !props.IsPoweredAttack())
            return 1m;
        return DynamicVars["PlayCountMultiplier"].BaseValue;
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);
        DynamicVars["PlayCountMultiplier"].BaseValue *= 2;
        EnergyCost.UpgradeBy(1);
    }
}