using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class SunbloomKindling : CollectorCardModel
{
    public SunbloomKindling() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CollectorKeyword.Smolder);
        WithKindle(5, 3);
        WithPower<StrengthPower>(2);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnSmolder(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}