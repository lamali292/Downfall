using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Basic;

[Pool(typeof(ChampCardPool))]
public class DefendChamp : ChampCardModel
{
    public DefendChamp() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}