using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.DynamicVars;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class CrushingGhostflame : GhostflameModel
{
    public override AbstractIntent Intent => new CustomAttackIntent(
        () => DynamicVars.GhostflameDamage,
        () => 2 * Repeat(GhostflameRepeatType.Damage)
    );

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GhostflameDamageVar(3)
    ];

    protected override int IgnitionRequirement => 2;

    public override FireColor FireColor => FireColor.Pink;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;

        var damage = DynamicVars.GhostflameDamage;
        var hitCount = 2 * Repeat(GhostflameRepeatType.Damage);

        await RepeatOnTargets(ctx, hitCount, GhostflameRepeatType.Damage,
            targets => CreatureCmd.Damage(ctx, targets, damage, DamageProps.nonCardUnpowered, Owner.Creature));
    }

    protected override int ProgressFrom(CardModel card, int energySpent)
    {
        return ProgressOnCardType(card, CardType.Skill);
    }
}