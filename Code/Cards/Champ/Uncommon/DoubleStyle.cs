using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class DoubleStyle : ChampCardModel
{
    public DoubleStyle() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
    }
    // TODO: Implement
}