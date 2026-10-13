using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>Slime counterpart of ShadowStepPower: at the start of the owner's next turn, becomes <see cref="SlimeDoubleDamagePower"/>.</summary>
public class SlimeDoubleDamageNextTurnPower : SlimeBossPowerModel
{
  public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return;
        await PowerCmd.Apply<SlimeDoubleDamagePower>(new ThrowingPlayerChoiceContext(), Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}
