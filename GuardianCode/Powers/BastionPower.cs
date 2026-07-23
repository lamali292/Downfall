using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Powers;


public class BastionPower : GuardianPowerModel, IAfterBrace
{
    public BastionPower()
    {
        WithTip(StaticHoverTip.Block);
        WithTip(GuardianTip.Brace);
    }


    public async Task AfterBrace(Player player, decimal amount)
    {
        if (player.Creature != Owner) return;
        var allies = CombatState.GetTeammatesOf(Owner).Where(e => e != Owner);
        foreach (var ally in allies)
        {
            await CreatureCmd.GainBlock(ally, Amount, ValueProp.Unpowered, null);
        }
        Flash();
    }
}