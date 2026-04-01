using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class FingerOfDeath : CollectorCardModel
{
    public FingerOfDeath() : base(4, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        
    }
    // TODO: Implement
}