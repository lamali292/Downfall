using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class Piledriver() : ChampCardModel(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    // TODO: Implement
}