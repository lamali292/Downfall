using Downfall.Code.Abstract;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Collector;

public class EmpowerPower : CollectorPowerModel, IHasSecondAmount
{
 
    public EmpowerPower()
    {
        WithVars(new IntVar("Turns", 2));
    }

    public void SetTurns(decimal turns)
    {
        DynamicVars["Turns"].BaseValue = turns;
        InvokeDisplayAmountChanged();
    }
    
    public string GetSecondAmount() => $"{DynamicVars["Turns"].BaseValue}";
    public override bool IsInstanced => true;
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, CombatState combatState)
    {
        if (player.Creature != Owner) return;
        DynamicVars["Turns"].UpgradeValueBy(-1);
        InvokeDisplayAmountChanged();
        await PowerCmd.Apply<StrengthPower>(Owner, Amount, Owner, null);
        if (DynamicVars["Turns"].BaseValue <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}