using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Powers;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class InfernoGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 3;
    public override FireColor FireColor => FireColor.Red;

    public override AbstractIntent Intent => new CustomAttackIntent(
        () => 4 + Intensity,
        () => HexaghostCmd.GetIgnitedCount(Owner) + (IsIgnited ? 0 : 1) + Repeat(GhostflameRepeatType.Damage)
    );

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (Owner.Creature.CombatState == null) return;
        var ignited = HexaghostCmd.GetIgnitedCount(Owner);
        var target = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;

        SfxCmd.Play("event:/sfx/characters/attack_fire");
        SpawnVfx(target);
        var hitCount = ignited + Repeat(GhostflameRepeatType.Damage);
        var damage = 4 + Intensity;
        for (var i = 0; i < hitCount; i++)
        {
            if (!target.IsHittable) continue;
            await CreatureCmd.Damage(ctx, target, damage, ValueProp.Move | ValueProp.Unpowered, Owner.Creature);
        }

        if (HexaghostCmd.AllIgnited(Owner))
            await PowerCmd.Apply<IntensityPower>(ctx, Owner.Creature, 2, Owner.Creature, null);
    }

    protected override async Task AfterEnergySpent(PlayerChoiceContext ctx, CardModel card, int amount)
    {
        if (!IsActive || card.Owner != Owner || LocalContext.NetId == null) return;
        if (!TryProgress(amount)) return;
        await Ignite(ctx);
    }
}