using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Collector;

public class FeelMyPainPower : CollectorPowerModel
{
    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner) return;
        var target = card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (target != null)
            await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, Amount,
                ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
    }
}