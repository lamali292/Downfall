using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Champ.Uncommon;

[Pool(typeof(ChampCardPool))]
public class Lariat : ChampCardModel
{
    public Lariat() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}