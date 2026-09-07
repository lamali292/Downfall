using BaseLib.Abstracts;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class EmpowerPower : CollectorPowerModel
{
    public EmpowerPower()
    {
        WithPower<StrengthPower>(0);
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    protected override int? SecondAmount => DynamicVars.Strength.IntValue;

    
    public void SetStrength(int amount)
    {
        DynamicVars.Strength.BaseValue = amount;
        this.InvokeSilentDisplayAmountChanged();
    }
    
 
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await MyCommonActions.ApplySelf<StrengthPower>(ctx, this);
        Flash();
        await PowerCmd.Decrement(this);
    }

}