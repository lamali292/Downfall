using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class Inscribe : AwakenedCardModel
{
    public Inscribe() : base(0, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
    }
    // TODO: Implement
}