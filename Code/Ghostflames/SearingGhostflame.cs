using BaseLib.Utils;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Events;
using Downfall.Code.Powers.Hexaghost;
using Downfall.Code.Vfx.Hexaghost;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Ghostflames;

public class SearingGhostflame : GhostflameModel
{
    protected override int IgnitionRequirement => 2;

    public override async Task OnIgnite()
    {
        var target = CombatState.HittableEnemies
            .TakeRandom(1, CombatState.RunState.Rng.CombatTargets).FirstOrDefault();
        if (target == null) return;
        if (Owner.Creature.CombatState == null) return;
        var intensity = DownfallHook.ModifyGhostflameEffectAdditive(Owner.Creature.CombatState, Owner, this);
        await CommonActions.Apply<SoulBurnPower>(target, null, 3 + intensity);
        await CommonActions.Apply<SoulBurnPower>(target, null, 3 + intensity);
    }

    public override NFire.FireColor FireColor => NFire.FireColor.Green;


    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsActive || cardPlay.Card.Owner != Owner || cardPlay.Card.Type != CardType.Attack) return;
        if (TryProgress())
            await Ignite();
    }
}