using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Awakened.Common;

[Pool(typeof(AwakenedCardPool))]
public class Initiation() : AwakenedCardModel(2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // TODO: Implement
}