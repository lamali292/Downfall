using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class Omen : CollectorCardModel
{
    public Omen() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}