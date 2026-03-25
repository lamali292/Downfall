using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Hexaghost.Basic;

[Pool(typeof(HexaghostCardPool))]
public class StrikeHexaghost() : HexaghostCardModel(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
{
    // TODO: Implement
}