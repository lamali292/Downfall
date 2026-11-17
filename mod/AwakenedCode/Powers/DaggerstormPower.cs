using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class DaggerstormPower : AwakenedPowerModel
{
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (player?.Creature != Owner) return;
        var enemy = card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (enemy == null) return;
        await CreatureCmd.Damage(new BlockingPlayerChoiceContext(), enemy, Amount,
            DamageProps.nonCardUnpowered, Owner, null, null);
    }
}