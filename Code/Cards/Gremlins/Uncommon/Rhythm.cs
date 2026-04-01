using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Rhythm : GremlinsCardModel
{
    public Rhythm() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}