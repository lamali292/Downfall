using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Revel : GremlinsCardModel
{
    public Revel() : base(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}