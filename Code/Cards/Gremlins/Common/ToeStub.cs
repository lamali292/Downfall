using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Common;

[Pool(typeof(GremlinsCardPool))]
public class ToeStub() : GremlinsCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // TODO: Implement
}