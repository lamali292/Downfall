using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class GigaBeam() : GuardianCardModel(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // TODO: Implement
}