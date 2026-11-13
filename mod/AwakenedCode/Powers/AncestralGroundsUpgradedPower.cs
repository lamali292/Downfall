using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Powers;

public class AncestralGroundsUpgradedPower : AwakenedPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.Player == null)
            return;
        await PlayerCmd.GainEnergy(3, Owner.Player);
        await DownfallCardCmd.GiveCard<Void>(Owner.Player, PileType.Draw, CardPilePosition.Top, animationTime: 0.2f);
        await PowerCmd.Decrement(this);
    }
}