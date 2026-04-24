using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class FeelMyPain : CollectorCardModel
{
    public FeelMyPain() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<FeelMyPainPower>(4, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FeelMyPainPower>(ctx, this);
    }
}