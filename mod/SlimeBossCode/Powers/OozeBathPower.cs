using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "While the enemy has Weak, they lose HP at the start of their turn and whenever you play a Status."
public class OozeBathPower : SlimeBossPowerModel
{
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    private bool TargetHasWeak => Owner.GetPowerInstances<WeakPower>().Any(w => w.Amount > 0);

    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || !TargetHasWeak) return;
        await CompatibilityCreatureCmd.Damage(new BlockingPlayerChoiceContext(), Owner, Amount,
            DamageProps.nonCardHpLoss, Applier, null, null);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Applier || cardPlay.Card.Type != CardType.Status || !TargetHasWeak)
            return;
        await CompatibilityCreatureCmd.Damage(ctx, Owner, Amount, DamageProps.nonCardHpLoss, Applier, null, null);
    }
}
