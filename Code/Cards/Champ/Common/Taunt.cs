using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class Taunt() : ChampCardModel(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    // TODO: Implement
}