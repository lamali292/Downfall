using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class SingularityShield : AwakenedCardModel
{
    public SingularityShield() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }
    // TODO: Implement
}