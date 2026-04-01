using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Common;

[Pool(typeof(SneckoCardPool))]
public class Cower : SneckoCardModel
{
    public Cower() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        
    }
    // TODO: Implement
}