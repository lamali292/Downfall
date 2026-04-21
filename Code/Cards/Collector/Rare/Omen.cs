using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class Omen : CollectorCardModel
{
    public Omen() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<OmenPower>(1);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<OmenPower>(this);
    }
}