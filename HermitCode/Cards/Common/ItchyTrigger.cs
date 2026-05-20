using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Common;

public sealed class ItchyTrigger : HermitCardModel, IHasDeadOnEffect
{
    public ItchyTrigger() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(7, 2);
        WithVar("CostReduction", 1, 1);
    }

   

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);
    }

    public Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        var cards =  Owner.PlayerCombatState?.Hand.Cards;
        var cardModel = cards?.Where(c => c.CostsEnergyOrStars(false))
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault() ?? 
                        cards?.Where(c => c.CostsEnergyOrStars(true))
            .TakeRandom(1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
        cardModel?.EnergyCost.AddThisTurn(-DynamicVars["CostReduction"].IntValue, true);
        return Task.CompletedTask;
    }
}