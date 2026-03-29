using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Champ.Token;

[Pool(typeof(TokenCardPool))]
public class Inspiration() : ChampCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    // TODO: Implement
}