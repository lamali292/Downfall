using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Basic;

[Pool(typeof(SneckoCardPool))]
public class StrikeSnecko() : SneckoCardModel(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    // TODO: Implement
}