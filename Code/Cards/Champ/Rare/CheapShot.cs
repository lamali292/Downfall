using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class CheapShot() : ChampCardModel(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    // TODO: Implement
}