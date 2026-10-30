using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Torchbearer : CollectorCardModel
{
    public Torchbearer() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithKindle(9, 3);
        WithTip(CollectorKeyword.Torchhead);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var torchhead = await TorchheadCmd.Kindle(ctx, this);
        var damage = torchhead.GetPower<TorchheadPower>()?.DynamicVars.TorchheadDamage.IntValue ?? 0;
        await TorchheadCmd.TorchheadAttack(Owner, damage, this, cardPlay).ExecuteIfPresent(ctx);
    }
}