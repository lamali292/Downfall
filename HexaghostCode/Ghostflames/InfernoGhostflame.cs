using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.Ghostflames.Intents;
using Hexaghost.HexaghostCode.Powers;
using Hexaghost.HexaghostCode.Vfx;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Ghostflames;

public class InfernoGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 3;
    public override FireColor FireColor => FireColor.Red;

    public override AbstractIntent Intent => new CustomAttackIntent(
        () => 4 + Intensity,
        () => HexaghostCmd.GetIgnitedCount(Owner) + (IsIgnited ? 0 : 1) * (1 + Repeat(GhostflameRepeatType.Damage))
    );

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (!TryBeginIgnite()) return;

        var damage = 4 + Intensity;
        var hitCount = HexaghostCmd.GetIgnitedCount(Owner) + Repeat(GhostflameRepeatType.Damage);

        await RepeatOnTargets(ctx, hitCount, GhostflameRepeatType.Damage,
            targets => CreatureCmd.Damage(ctx, targets, damage, ValueProp.Unpowered, Owner.Creature));

        if (HexaghostCmd.AllIgnited(Owner))
            await PowerCmd.Apply<IntensityPower>(ctx, Owner.Creature, 2, Owner.Creature, null);

        await Cmd.Wait(0.2f);
        await HexaghostCmd.ExtinguishAllExceptThis(ctx, Owner, this);
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (!participants.Contains(Owner.Creature) || !IsIgnited) return Task.CompletedTask;
        Extinguish();
        HexaghostVisualsBridge.Refresh(Owner);
        return Task.CompletedTask;
    }

    protected override async Task AfterEnergySpent(PlayerChoiceContext ctx, CardModel card, int amount)
    {
        if (!IsActive || card.Owner != Owner) return;
        if (!TryProgress(amount)) return;
        await Ignite(ctx);
    }
}