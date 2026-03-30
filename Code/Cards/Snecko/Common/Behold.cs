using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Common;

[Pool(typeof(SneckoCardPool))]
public class Behold() : SneckoCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // TODO: Implement
}