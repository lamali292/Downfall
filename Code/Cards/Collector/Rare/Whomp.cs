using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class Whomp() : CollectorCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // TODO: Implement
}