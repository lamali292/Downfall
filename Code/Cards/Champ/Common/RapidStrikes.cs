using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class RapidStrikes : ChampCardModel
{
    public RapidStrikes() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}