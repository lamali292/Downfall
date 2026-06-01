using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class ProtectingCall : CollectorCardModel
{
    public ProtectingCall() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithVars(new SummonVar(6).WithUpgrade(2));
        WithPower<ProtectingCallPower>(2, 1);
        WithTip(CollectorTip.Kindle);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var torchhead = await CollectorCmd.SummonTorchhead(ctx, Owner, DynamicVars.Summon.IntValue, this);
        await CommonActions.Apply<ProtectingCallPower>(ctx, torchhead, this);
    }
}