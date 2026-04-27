using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Powers;

public class AncestralGroundsPower : AwakenedPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null)
            return;
        await PlayerCmd.GainEnergy(2, Owner.Player);
        await DownfallCardCmd.GiveCard<Void>(Owner.Player, PileType.Draw, CardPilePosition.Top, animationTime: 0.2f);
        await PowerCmd.Decrement(this);
    }
}