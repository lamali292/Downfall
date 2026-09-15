using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Requiem : CollectorCardModel
{
    public Requiem() : base(3, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<RequiemPower>(1, false);
        WithTip(CollectorTip.Pyred);
        WithTip(CollectorKeyword.Pyre);
        WithTip(CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RequiemPower>(ctx, this);
    }
}