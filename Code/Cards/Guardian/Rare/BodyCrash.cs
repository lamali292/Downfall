using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class BodyCrash() : GuardianCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // TODO: Implement
}