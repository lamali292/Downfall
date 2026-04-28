using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class HereAndNowPower() : HexaghostPowerModel(PowerType.Debuff, PowerStackType.Single)
{
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        await HexaghostCmd.Extinguish(Owner.Player);
    }
}