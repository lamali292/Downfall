using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Hexaghost;

public class PoltergeistPower : HexaghostPowerModel
{
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner) return;
        var creature = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (creature == null) return;
        await CreatureCmd.Damage(ctx, creature, Amount,
            ValueProp.Unblockable | ValueProp.Unpowered, Owner, null);
        Flash();
    }
}