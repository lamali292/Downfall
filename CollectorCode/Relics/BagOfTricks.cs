using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BagOfTricks : CollectorRelicModel, IOnPyre
{
    public BagOfTricks() : base(RelicRarity.Common)
    {
        WithVar("MaxUses", 3);
        WithVar("UsesLeft", 3);
    }
    private DynamicVar MaxUses => DynamicVars["MaxUses"];
    private DynamicVar UsesLeft => DynamicVars["UsesLeft"];
    public override int DisplayAmount => UsesLeft.IntValue;
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    
    public async Task OnPyre(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (!CombatManager.Instance.IsInProgress && !Owner.Creature.IsDead)//If pyre resolves as or shorty after combat ends or as a player dies, don't draw cards (MP).
        {
            return;
        }
        
        if (UsesLeft.BaseValue > 0)
        {
            UsesLeft.BaseValue--;
            CardModel cardModel = await CardPileCmd.Draw(ctx, Owner);
        }
        Flash();
        InvokeDisplayAmountChanged();
    }
    
    public override Task BeforeCombatStart()
    {
        UsesLeft.BaseValue = MaxUses.BaseValue;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}