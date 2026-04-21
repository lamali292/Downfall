using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Guardian;

public class FloatingOrbsPower : GuardianPowerModel
{
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player) return;

        if (cardPlay.Resources.EnergySpent != 0 || cardPlay.Resources.StarsSpent != 0) return;
        var target = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        await CreatureCmd.Damage(choiceContext, target, Amount, ValueProp.Move | ValueProp.Unpowered, Owner);
    }
}