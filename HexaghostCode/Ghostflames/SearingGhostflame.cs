using BaseLib.Utils;
using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        var target = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (target == null) return;
        if (Owner.Creature.CombatState == null) return;

        var intensity = Intensity;
        var repeat = 2 + Repeat(GhostflameRepeatType.Soulburn);

        SfxCmd.Play("event:/sfx/characters/attack_fire");
        SpawnVfx(target);

        for (var i = 0; i < repeat; i++) await PowerCmd.Apply<SoulBurnPower>(ctx, target, 3 + intensity, Owner.Creature, null);
    }


    protected override async Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner ||
            LocalContext.NetId == null) return;
        var shouldCount = HexaghostHook.GhostflameConditionOverwrites(CombatState, Owner, this, cardPlay);
        if (!(cardPlay.Card.Type == CardType.Attack || shouldCount)) return;
        if (!TryProgress()) return;
        await Ignite(ctx);
    }
}