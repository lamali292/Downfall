using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class DemonGlyph : AwakenedCardModel
{
    public DemonGlyph() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
    }

    // TODO: Implement
}