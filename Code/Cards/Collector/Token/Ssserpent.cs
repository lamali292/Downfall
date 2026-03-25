using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Collector.Token;

[Pool(typeof(TokenCardPool))]
public class Ssserpent() : CollectorCardModel(1, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // TODO: Implement
}