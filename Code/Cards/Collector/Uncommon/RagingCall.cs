using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RagingCall : CollectorCardModel
{
    public RagingCall() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithVars(new SummonVar(6).WithUpgrade(1));
        WithPower<RagingCallPower>(3, 5);
        WithTip(DownfallTip.Kindle);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var torchhead = await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
        await CommonActions.Apply<RagingCallPower>(torchhead, this);
    }
    
}