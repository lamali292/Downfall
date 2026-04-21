using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Collector.Basic;

[Pool(typeof(CollectorCardPool))]
public class FuelTheFire : CollectorCardModel
{
    public FuelTheFire() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithPower<ReserveNextTurnPower>(2);
        WithPyre();
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ReserveNextTurnPower>(this);
        if (IsUpgraded) await CommonActions.ApplySelf<DrawCardsNextTurnPower>(this, 1);
    }
}