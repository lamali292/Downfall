using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class SearingGhostflame : GhostflameModel
{
    public override int IgnitionRequirement => 2;

    public override FireColor FireColor => FireColor.Yellow;

    public override AbstractIntent Intent => new MultiStatusIntent<SoulBurnPower>(
        () => 3 + Intensity,
        2 * (1 + Repeat(GhostflameRepeatType.Soulburn))
    );

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;
        var soulburn = 3 + Intensity;
        var repeat = 2 + Repeat(GhostflameRepeatType.Soulburn);
        await RepeatOnTargets(ctx, repeat, GhostflameRepeatType.Soulburn,
            targets => PowerCmd.Apply<SoulBurnPower>(ctx, targets, soulburn, Owner.Creature, null));
    }

    protected override Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
        => TriggerOnCardType(ctx, cardPlay, CardType.Attack);
}