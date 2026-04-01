using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Basic;

[Pool(typeof(GremlinsCardPool))]
public class DefendGremlins : GremlinsCardModel
{
    public DefendGremlins() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}