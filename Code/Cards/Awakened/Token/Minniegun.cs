using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Awakened.Token;

[Pool(typeof(TokenCardPool))]
public class Minniegun() : AwakenedCardModel(2, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    // TODO: Implement
}