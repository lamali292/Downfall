using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Basic;

[Pool(typeof(GremlinsCardPool))]
public class TagTeam : GremlinsCardModel
{
    public TagTeam() : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}