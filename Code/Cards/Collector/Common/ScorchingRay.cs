using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class ScorchingRay : CollectorCardModel
{
    public ScorchingRay() : base(0, CardType.Attack, CardRarity.Common, TargetType.RandomEnemy)
    {
        WithDamage(8, 3);
    }

    protected override bool HasEnergyCostX => true;
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var amount = ResolveEnergyXValue();
        await CommonActions.CardAttack(this, cardPlay, amount).Execute(ctx);
    }
    
}