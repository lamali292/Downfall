using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class DaggerstormPower : AwakenedPowerModel
{
    protected override async Task AfterCardGeneratedForCombat(PlayerChoiceContext ctx, CardModel card, Player? player)
    {
        if (card.Owner.Creature != Owner) return;
        var enemy = card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.Enemies);
        if (enemy == null) return;
        await CreatureCmd.Damage(ctx, enemy, Amount, ValueProp.Unpowered, Owner, null);
    }
}