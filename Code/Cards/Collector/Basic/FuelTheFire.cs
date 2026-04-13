using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Core.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Basic;

[Pool(typeof(CollectorCardPool))]
public class FuelTheFire : CollectorCardModel
{
    public FuelTheFire() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        CollectorEnergy.Gain(cardPlay.Card.Owner, 1);
    }


    protected override void OnUpgrade()
    {
    }
}