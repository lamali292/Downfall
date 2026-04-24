using BaseLib.Hooks;
using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Powers.Hexaghost;

public class SoulBurnPower : HexaghostPowerModel, IHasSecondAmount
{

    public SoulBurnPower()
    {
        WithVar("Turns", 3);
    }
    
    
    public override IEnumerable<HealthBarForecastSegment> GetHealthBarForecastSegments(HealthBarForecastContext ctx)
    {
        if (Amount <= 0) yield break;
        if (DynamicVars["Turns"].BaseValue != 1) yield break;
        yield return new HealthBarForecastSegment(
            Amount,
            new Color("8AD974"),
            HealthBarForecastDirection.FromRight,
            2
        );
    }

    public string GetSecondAmount()
    {
        return $"{DynamicVars["Turns"].BaseValue}";
    }

    public override async Task AfterSideTurnStart(CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side)
            return;
        DynamicVars["Turns"].UpgradeValueBy(-1);
        InvokeDisplayAmountChanged();
        if (DynamicVars["Turns"].BaseValue > 0) return;
        await Detonate();
    }

    public async Task Detonate( Creature? applier = null, bool keepOne = false)
    {
        await CreatureCmd.Damage(new ThrowingPlayerChoiceContext(), Owner, keepOne ? Amount - 1 : Amount, ValueProp.Unblockable | ValueProp.Unpowered, null, null);
        if (keepOne)
        {
            await PowerCmd.ModifyAmount(new ThrowingPlayerChoiceContext(), this, 1-Amount, applier, null);
        }
        else
        {
            await PowerCmd.Remove(this);
        }
        
        await Cmd.CustomScaledWait(0.1f, 0.25f);
    }
}