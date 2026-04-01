using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Common;

[Pool(typeof(SneckoCardPool))]
public class Nope : SneckoCardModel
{
    public Nope() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        
    }
    // TODO: Implement
}