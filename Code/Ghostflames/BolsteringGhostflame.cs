using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Ghostflames;

public class BolsteringGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 1;

    public override NFire.FireColor FireColor => NFire.FireColor.Blue;

    public override async Task OnIgnite(PlayerChoiceContext ctx)
    {
        if (Owner.Creature.CombatState == null) return;
        var intensity = DownfallHook.ModifyGhostflameEffectAdditive(Owner.Creature.CombatState, Owner, this);
        await CreatureCmd.GainBlock(Owner.Creature, 4 + intensity, ValueProp.Move | ValueProp.Unpowered, null);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner.Creature, 1, Owner.Creature, null);
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Power ||
            LocalContext.NetId == null) return;
        var ctx = new HookPlayerChoiceContext(
            Owner,
            LocalContext.NetId.Value,
            GameActionType.Combat);
        if (TryProgress())
            await Ignite(ctx);
    }
}