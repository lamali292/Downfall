using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Powers;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class SearingGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 2;

    public override FireColor FireColor => FireColor.Green;
    public override AbstractIntent Intent => new MultiStatusIntent<SoulBurnPower>(
        () => 3 + Intensity,
        2 + Repeat(GhostflameRepeatType.Soulburn)
    );
    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        var target = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        if (Owner.Creature.CombatState == null) return;
        
        var intensity = Intensity;
        var repeat = 2 + Repeat(GhostflameRepeatType.Soulburn);
        
        SfxCmd.Play("event:/sfx/characters/attack_fire");
        SpawnVfx(target);
        
        for (var i = 0; i < repeat; i++)
        {
            await CommonActions.Apply<SoulBurnPower>(ctx, target, null, 3 + intensity);
        }
    }


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner ||
            LocalContext.NetId == null) return;
        var shouldCount = HexaghostHook.GhostflameConditionOverwrites(CombatState, Owner, this, cardPlay);
        if (!(cardPlay.Card.Type == CardType.Attack || shouldCount)) return;
        if (!TryProgress()) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        var task = Ignite(ctx);
        await ctx.AssignTaskAndWaitForPauseOrCompletion(task);
    }
}