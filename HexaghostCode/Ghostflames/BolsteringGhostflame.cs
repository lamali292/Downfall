using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class BolsteringGhostflame : GhostflameModel
{
    public override AbstractIntent Intent => new BolsteringIntent();
    public override int IgnitionRequirement => 1;

    public override FireColor FireColor => FireColor.Blue;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;

        var block = 4 + Intensity;
        var repeat = 1 + Repeat(GhostflameRepeatType.Block);
        for (var i = 0; i < repeat; i++)
            await CreatureCmd.GainBlock(Owner.Creature, block, BlockProps.nonCardUnpowered, null);

        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, 1, Owner.Creature, null);
    }

    protected override Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
        => TriggerOnCardType(ctx, cardPlay, CardType.Power);
}