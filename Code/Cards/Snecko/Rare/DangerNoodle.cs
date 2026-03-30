using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Rare;

[Pool(typeof(SneckoCardPool))]
public class DangerNoodle() : SneckoCardModel(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // TODO: Implement
}