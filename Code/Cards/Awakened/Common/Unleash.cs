using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Unleash : AwakenedCardModel
{
    public Unleash() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }
    // TODO: Implement
}