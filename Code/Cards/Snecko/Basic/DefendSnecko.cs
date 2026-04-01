using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Basic;

[Pool(typeof(SneckoCardPool))]
public class DefendSnecko : SneckoCardModel
{
    public DefendSnecko() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}