using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Awakened.AwakenedCode.Powers;

public class DaggerstormPower : AwakenedPowerModel
{
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? player)
    {
        if (card.Owner.Creature != Owner || LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            card.Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        var enemy = card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.Enemies);
        if (enemy == null) return;
        await CreatureCmd.Damage(ctx, enemy, Amount, ValueProp.Unpowered, Owner, null);
    }
}