using BaseLib.Patches.Localization;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Powers.Abstract;

public abstract class PowerNextTurn<T> : DownfallPowerModel, IAddDumbVariablesToPowerDescription
    where T : PowerModel
{
    protected PowerNextTurn()
    {
        WithTips(e => ModelDb.Power<T>().HoverTips);
    }

    public override bool AllowNegative => ModelDb.Power<T>().AllowNegative;
    public override PowerType Type => ModelDb.Power<T>().Type;
    public override PowerStackType StackType => ModelDb.Power<T>().StackType;
    public override PowerInstanceType InstanceType => ModelDb.Power<T>().InstanceType;

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        description.Add("NAmount", -Amount);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<T>(ctx, Owner, Amount, Applier, null);
    }
}