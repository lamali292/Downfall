using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class CrushingGhostflame : GhostflameModel
{
    public override AbstractIntent Intent => new CustomAttackIntent(
        () => 3 + Intensity,
        () => 2 * (1 + Repeat(GhostflameRepeatType.Damage))
    );

    public override int IgnitionRequirement => 2;

    public override FireColor FireColor => FireColor.Pink;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;

        var damage = 3 + Intensity;
        var hitCount = 2 + Repeat(GhostflameRepeatType.Damage);

        await RepeatOnTargets(ctx, hitCount, GhostflameRepeatType.Damage,
            targets => CreatureCmd.Damage(ctx, targets, damage, DamageProps.nonCardUnpowered, Owner.Creature));
    }

    protected override Task BeforeCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
        => TriggerOnCardType(ctx, cardPlay, CardType.Skill);
}