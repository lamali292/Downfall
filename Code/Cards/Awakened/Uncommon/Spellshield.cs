using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Spellshield : AwakenedCardModel
{
    public Spellshield() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
    }
    // TODO: Implement
}