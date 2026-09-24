using BaseLib.Patches.Localization;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class StrengthNextTurnPower : SlimeBossPowerModel, IAddDumbVariablesToPowerDescription
{
    public StrengthNextTurnPower()
    {
        WithTips(e => ModelDb.Power<StrengthPower>().HoverTips);
    }

    public override bool AllowNegative => ModelDb.Power<StrengthPower>().AllowNegative;
    public override PowerType Type => ModelDb.Power<StrengthPower>().Type;
    public override PowerStackType StackType => ModelDb.Power<StrengthPower>().StackType;
    public override PowerInstanceType InstanceType => ModelDb.Power<StrengthPower>().InstanceType;

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("NAmount", -Amount);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Applier, null);
    }
}
