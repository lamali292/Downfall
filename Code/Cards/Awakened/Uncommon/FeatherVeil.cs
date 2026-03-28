using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class FeatherVeil : AwakenedCardModel
{
    public FeatherVeil() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }
    // TODO: Implement
}