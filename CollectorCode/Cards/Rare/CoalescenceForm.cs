using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class CoalescenceForm : CollectorCardModel
{
    public CoalescenceForm() : base(5, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithReserve(1);
        WithPower<CoalescenceFormPower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CollectorCmd.GetReserve(this);
        await CommonActions.ApplySelf<CoalescenceFormPower>(ctx, this);
    }

}