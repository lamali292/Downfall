using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Common;

[Pool(typeof(ChampCardPool))]
public class PerfecterStrike : ChampCardModel
{
    public PerfecterStrike() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}