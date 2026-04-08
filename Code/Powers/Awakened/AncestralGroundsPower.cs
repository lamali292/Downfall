using Downfall.Code.Abstract;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Powers.Awakened;

public class AncestralGroundsPower : AwakenedPowerModel
{
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null)
            return;
        await PlayerCmd.GainEnergy(2, Owner.Player);
        await DownfallCardCmd.GiveCard<Void>(Owner.Player, PileType.Draw, CardPilePosition.Top, 0.2f);
        await PowerCmd.Decrement(this);
    }
}