using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Rare;

[Pool(typeof(GremlinsCardPool))]
public class Necromancy : GremlinsCardModel
{
    public Necromancy() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        
    }
    // TODO: Implement
}