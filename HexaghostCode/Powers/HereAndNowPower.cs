using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class HereAndNowPower : HexaghostPowerModel
{
    public HereAndNowPower() : base(PowerType.Debuff, PowerStackType.Single)
    {
        WithTip(HexaghostTip.Extinguish);
    }
    
    
    
    public override async Task BeforeSideTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        await HexaghostCmd.Extinguish(Owner.Player);
    }
}