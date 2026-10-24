using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class SoulfirePower : HexaghostPowerModel
{
    public SoulfirePower()
    {
        WithTip<SoulBurnPower>();
    }
    
    
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (applier != Owner || cardSource == null || amount <= 0 || power is not SoulBurnPower) return;
        await CreatureCmd.GainBlock(Owner, Amount, BlockProps.nonCardUnpowered, null);
        Flash();
    }
}