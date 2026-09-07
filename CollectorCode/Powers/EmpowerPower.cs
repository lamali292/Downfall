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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Powers;

public class EmpowerPower : CollectorPowerModel
{
    public EmpowerPower()
    {
        WithCards(0);
        WithCardTip<Ember>(Upgrade);
    }

    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    protected override int? SecondAmount => DynamicVars.Cards.IntValue;
    
    public void SetCards(int amount)
    {
        DynamicVars.Cards.BaseValue = amount;
        this.InvokeSilentDisplayAmountChanged();
    }
    
    private void Upgrade(Ember obj) => Upgrade(obj, this);
    private static void Upgrade(Ember obj, PowerModel power)
    {
        for (var i = 0; i < power.DynamicVars.Cards.BaseValue; i++)
        {
            CardCmd.Upgrade(obj);
        }
    }
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await DownfallCardCmd.GiveCard<Ember>(player, PileType.Hand, action: Upgrade);
        //await MyCommonActions.ApplySelf<StrengthPower>(ctx, this);
        Flash();
        await PowerCmd.Decrement(this);
    }

  
}