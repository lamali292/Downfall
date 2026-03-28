using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class FourthDimension : AwakenedCardModel
{
    public FourthDimension() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
    }

    // TODO: Implement
}