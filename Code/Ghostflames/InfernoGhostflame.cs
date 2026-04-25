using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using Downfall.Code.Powers.Hexaghost;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Ghostflames;

public class InfernoGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 3;
    public override NFire.FireColor FireColor => NFire.FireColor.Red;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        var target = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        var ignited = HexaghostCmd.GetIgnitedCount(Owner);
        if (Owner.Creature.CombatState == null) return;
        var intensity = DownfallHook.ModifyGhostflameEffectAdditive(Owner.Creature.CombatState, Owner, this);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, (4 + intensity) * ignited,
            ValueProp.Move | ValueProp.Unpowered, null, null);
        if (ignited >= 6)
            await PowerCmd.Apply<IntensityPower>(ctx, Owner.Creature, 1, Owner.Creature, null);
    }

    public override async Task AfterEnergySpent(CardModel card, int amount)
    {
        if (!IsActive || card.Owner != Owner || LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        if (TryProgress())
            await Ignite(ctx);
    }
}