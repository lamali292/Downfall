using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Powers;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class InfernoGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 3;
    public override NFire.FireColor FireColor => NFire.FireColor.Red;
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
        
        var attack = new AttackCommand(4 + Intensity)
        {
            Attacker = Owner.Creature
        };
        await attack.WithHitCount(ignited + Repeat(GhostflameRepeatType.Damage)).Targeting(target).Execute(ctx);
        
        if (HexaghostCmd.AllIgnited(Owner))
            await PowerCmd.Apply<IntensityPower>(ctx, Owner.Creature, 2, Owner.Creature, null);
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