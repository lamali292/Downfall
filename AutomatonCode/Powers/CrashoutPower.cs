using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class CrashoutPower : AutomatonPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player.Creature != Owner || cardPlay.Card.Type != CardType.Status) return;
        var enemy = Owner.CombatState?.HittableEnemies
            .TakeRandom(1, player.RunState.Rng.CombatTargets).FirstOrDefault();
        if (enemy == null) return;
        await CreatureCmd.Damage(ctx, enemy, Amount, ValueProp.Unpowered, Owner, null);
    }
}