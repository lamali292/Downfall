using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class RagingCall : CollectorCardModel
{
    public RagingCall() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKindle(6, 1);
        WithPower<RagingCallPower>(3, 5, false);
        WithTip(CollectorTip.Kindle);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var torchhead = await CollectorCmd.Kindle(ctx,this);
        await CommonActions.Apply<RagingCallPower>(ctx, torchhead, this);
    }
}