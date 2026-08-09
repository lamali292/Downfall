using Automaton.AutomatonCode.Core;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Automaton.AutomatonCode.Powers;

public class CrashoutPower : AutomatonPowerModel
{
    public override async Task AfterCardPlayedLate(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (player.Creature != Owner || cardPlay.Card.Type != CardType.Status) return;
        var enemy = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (enemy == null) return;
        await DownfallCreatureCmd.Damage(ctx, enemy, Amount, DamageProps.nonCardUnpowered, Owner, null, null);
    }
}