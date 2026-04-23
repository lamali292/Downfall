using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using Downfall.Code.Powers.Hexaghost;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Ghostflames;

public class CrushingGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 2;
    public override async Task OnIgnite()
    {
        var target = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        if (Owner.Creature.CombatState == null) return;
        var intensity = DownfallHook.ModifyGhostflameEffectAdditive(Owner.Creature.CombatState, Owner, this);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, 3 + intensity, ValueProp.Move | ValueProp.Unpowered, null, null);
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), target, 3 + intensity, ValueProp.Move | ValueProp.Unpowered, null, null);
    }
    
    public override NFire.FireColor FireColor => NFire.FireColor.Pink;
    
    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Skill) return;
        if (TryProgress())
            await Ignite();
    }
}