using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class BagOfTricks : CollectorRelicModel, IAfterCardPyred
{
    public BagOfTricks() : base(RelicRarity.Common)
    {
        WithVar("MaxUses", 3);
        WithVar("UsesLeft", 3);
        WithPower<DrawCardsNextTurnPower>(1, false);
        WithCards(1);
        WithTip(CollectorKeyword.Pyre);
    }
    private DynamicVar MaxUses => DynamicVars["MaxUses"];
    private DynamicVar UsesLeft => DynamicVars["UsesLeft"];
    public override int DisplayAmount => UsesLeft.IntValue;
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    
    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        if (card.Owner != Owner) return;
        if (UsesLeft.BaseValue <= 0) return;
        UsesLeft.BaseValue--;
        //await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue,  Owner);
        await MyCommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
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