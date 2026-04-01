using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Billow : CollectorCardModel
{
    public Billow() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}