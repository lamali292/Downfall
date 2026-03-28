using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Spew : AwakenedCardModel
{
    public Spew() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }
    // TODO: Implement
}