using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you apply a new debuff to an enemy, gain 3 Block.
/// </summary>
public sealed class RedScarf : HermitRelicModel
{
    public RedScarf() : base(RelicRarity.Common)
    {
        WithBlock(3);
    }


    public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target,
        Creature? applier, CardModel? cardSource)
    {
        if (amount != 0 && target.IsEnemy && power.GetTypeForAmount(amount) == PowerType.Debuff &&
            (target.GetPower(power.Id)?.Amount ?? 0) == 0 && applier == Owner.Creature)
        {
            Flash();
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
        }
    }
}