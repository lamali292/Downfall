using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Whenever you apply a new debuff to an enemy, gain 3 Block.
/// </summary>
public sealed class RedScarf : HermitRelicModel
{
    public RedScarf() : base(RelicRarity.Rare)
    {
        WithBlock(3);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (applier != Owner.Creature || !power.IsNewEnemyDebuff(amount)) return;

        Flash();
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, null);
    }
}
