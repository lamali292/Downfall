using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class TemporalStrike() : GuardianCardModel(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // TODO: Implement
}