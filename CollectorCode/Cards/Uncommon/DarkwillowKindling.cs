using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Patches;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class DarkwillowKindling : CollectorCardModel
{
    public DarkwillowKindling() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        //WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithKeyword(CollectorKeyword.Smolder);
        WithPower<ReserveNextTurnPower>(1, false);
        WithReserveTip();
        WithKindle(3, 3);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    protected override async Task OnSmolder(PlayerChoiceContext ctx)
    {
        await TorchheadCmd.Kindle(ctx, this);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this);
    }
}