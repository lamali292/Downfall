using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class Execute : ChampCardModel
{
    public Execute() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}