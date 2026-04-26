using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.Code.Cards.Automaton.Basic;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using Downfall.Code.Ghostflames.Intents;
using Downfall.Code.Powers.Hexaghost;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.Code.Ghostflames;

public class SearingGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 2;

    public override NFire.FireColor FireColor => NFire.FireColor.Green;
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
        if (!IsActive || cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack ||
            LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        if (TryProgress())
            await Ignite(ctx);
    }
}