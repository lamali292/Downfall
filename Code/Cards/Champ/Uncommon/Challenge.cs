using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Challenge() : ChampCardModel(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    // TODO: Implement
}