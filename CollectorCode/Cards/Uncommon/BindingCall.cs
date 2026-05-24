using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class BindingCall : CollectorCardModel
{
    public BindingCall() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithVars(new SummonVar(6).WithUpgrade(2));
        WithPower<BindingCallPower>(2, 1);
        WithTip(CollectorTip.Kindle);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var torchhead = await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
        await CommonActions.Apply<BindingCallPower>(ctx, torchhead, this);
    }
}