using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class MakeshiftBlade : SneckoCardModel
{
    public MakeshiftBlade() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}