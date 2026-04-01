using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Common;

[Pool(typeof(GremlinsCardPool))]
public class Patsy : GremlinsCardModel
{
    public Patsy() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        
    }
    // TODO: Implement
}