using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

// Amount is a percentage (25/50), not a fraction - WithPower<T> only accepts int base/upgrade values.
public class OpeningTackleSlimeBoostPower : SlimeBossPowerModel, IModifyDamageMultiplicative
{
    public decimal ModifyDamageMultiplicativeCompability(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        return dealer?.Monster is SlimeModel slime && slime.PetOwner == Owner ? 1 + Amount / 100M : 1M;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (participants.Contains(Owner)) await PowerCmd.Remove(this);
    }
}
