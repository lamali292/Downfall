using Awakened.AwakenedCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Powers;

public class AncestralGroundsPower : AwakenedPowerModel
{
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null)
            return;
        await PlayerCmd.GainEnergy(2, Owner.Player);
        await DownfallCardCmd.GiveCard<Void>(Owner.Player, PileType.Draw, CardPilePosition.Top, animationTime: 0.2f);
        await PowerCmd.Decrement(this);
    }
}