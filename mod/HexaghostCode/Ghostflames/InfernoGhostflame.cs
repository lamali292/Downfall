using Downfall.DownfallCode.Commands;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.DynamicVars;
using Hexaghost.HexaghostCode.Extensions;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Powers;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class InfernoGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 3;
    public override FireColor FireColor => FireColor.Red;

    public override AbstractIntent Intent => new CustomAttackIntent(
        () => DynamicVars.GhostflameDamage,
        () => (HexaghostCmd.GetIgnitedCount(Owner) + (IsIgnited ? 0 : 1)) * Repeat(GhostflameRepeatType.Damage)
    );

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new GhostflameDamageVar(4),
        new PowerVar<IntensityPower>(2)
    ];


    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<IntensityPower>()
    ];

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;

        var damage = DynamicVars.GhostflameDamage;
        var hitCount = HexaghostCmd.GetIgnitedCount(Owner) * Repeat(GhostflameRepeatType.Damage);

        await RepeatOnTargets(ctx, hitCount, GhostflameRepeatType.Damage,
            targets => CreatureCmd.Damage(ctx, targets, damage, DamageProps.nonCardUnpowered, Owner.Creature));

        if (HexaghostCmd.AllIgnited(Owner))
            for (var i = 0; i < Repeat(GhostflameRepeatType.Intensity); i++)
                await MyCommonActions.ApplySelf<IntensityPower>(ctx, this);

        await Cmd.Wait(0.2f);
        await HexaghostCmd.ExtinguishAllExceptThis(ctx, Owner, this);
    }

    public override Task AfterSideTurnEndLate(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature) || !IsIgnited) return Task.CompletedTask;
        Extinguish();
        HexaghostCmd.Refresh(Owner);
        return Task.CompletedTask;
    }

    // Progress is the energy spent, so 'primed' for a card is its energy cost.
    protected override int ProgressFrom(CardModel card, int energySpent)
    {
        if (card.Owner != Owner) return 0;
        return energySpent;
    }
}