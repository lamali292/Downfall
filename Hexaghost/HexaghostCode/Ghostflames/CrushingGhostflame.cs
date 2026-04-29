using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class CrushingGhostflame : GhostflameModel
{
    public override AbstractIntent Intent => new CustomAttackIntent(
        () => 3 + Intensity,
        () => 2 + Repeat(GhostflameRepeatType.Damage)
    );
    
    protected override int IgnitionRequirement => 2;

    public override FireColor FireColor => FireColor.Pink;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        var target = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        if (Owner.Creature.CombatState == null) return;
        
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        SpawnVfx(target);
        
        var hitCount = 2 + Repeat(GhostflameRepeatType.Damage);
        var damage = 3 + Intensity;
        for (var i = 0; i < hitCount; i++)
        {
            if (!target.IsHittable) continue;
            await CreatureCmd.Damage(ctx, target, damage, ValueProp.Move | ValueProp.Unpowered, Owner.Creature);
        }
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner ||
            LocalContext.NetId == null) return;
        var shouldCount = HexaghostHook.GhostflameConditionOverwrites(CombatState, Owner, this, cardPlay);
        if (!(cardPlay.Card.Type == CardType.Skill || shouldCount)) return;
        if (!TryProgress()) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        var task = Ignite(ctx);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(task);
    }
    
}


